# iov42 Core SDK

This repository contains the iov42 core SDK for C#/.NET, that can be used to interact with the iov42 platform.

Below you can find instructions on how to use this library. For additional information, including description of all public methods with examples, visit our Wiki.

## Installation

The simplest way to use the library is to clone the repository and then load it into Visual Studio (Community Edition or higher). You can also use Visual Studio Code with the correct extensions.

## Running the tests

In Visual Studio Community Edition, load the solution file. To run the integration tests, either edit the Environment property in Support\TestEnvironment.cs to point to the correct environment url or set an environment variable "IOV42_ENVIRONMENT" with the url. 

You can run the tests by right clicking on the project and selecting Run Tests.

## Using the SDK in an application

Currently there are two ways to do this.

1. Build the libraries and include them directly in your project.
2. Include the iov42sdk and BouncyCastleCrypto source code in your solution.

We plan to add a nuget package at a later date.

### Creating an identity

To be able to talk to the platform we need to use an identity and to create an identity we use the IdentityBuilder.

``` csharp
var identityBuilder = new IdentityBuilder(k => new BouncyCrypto());
```

This creates an IdentityBuilder and it will use the BouncyCastle crypto implementation to handle the cryptography for identities created with this builder.

``` csharp
var identity = identityBuilder.Create();
```

This creates a new identity with a randomly created id and a new key pair. Note - this has not been saved to the platform yet (that's the next step).

You can also use an existing identity by passing the id and the keypair to use.

### Creating the connection

To interact with the platform you need to create a client. To do this, call the ClientBuilder and pass the url for the endpoint and the identity you just created. If it is a new identity that has not been saved to the platform before then you use:

``` csharp
var client = await ClientBuilder.CreateWithNewIdentity("a url here", identity);
```

If you plan to use an entry that has been previously saved then you can use:

``` csharp
var client = await ClientBuilder.CreateWithExistingIdentity("a url here", identity);
```

You can now interact with the platform using this client!

### Getting the platform status

The first thing you might want to do is check that the platform is running correctly:

``` csharp
var result = await client.GetHealthStatus();
```

If the result.Success is true then the call was successful. You can then examine the current build number and the status of all the components. If the result.Success was false then you can examine the Reason, StatusCode and the Errors in the response to determine why.

You can also get information about a specific node including it's public key using:

``` csharp
var result = await client.GetNodeInfo();
```


### Creating another identity

You can create another identity using some of the calls you did above. 

``` csharp
var newIdentity = identityBuilder.Create();
var issueIdentityResponse = await client.CreateIdentity(newIdentity);
```
This will create a new identity on the platform using the identifier and keypair in newIdentity. If you wanted to switch to using this identity for calls to the platform you would need to create a new client passing this identity.

### Create an asset type

There are two main types of asset on the platform - unique and quantifiable.

To create a new unique asset type:

``` csharp
var horseId = "horse";
var newUniqueAssetTypeResponse = await client.CreateUniqueAssetType(horseId);
```

To create a new quantifiable asset type:

``` csharp
var eGbpId = "eGBP";
var numberOfDecimalPlaces = 2;
var newQuantifiableAssetTypeResponse = await client.CreateQuantifiableAssetType(eGbpId, numberOfDecimalPlaces);
```
The scale is the number of decimal places to support.

### Creating Assets

Now there are asset types we can create actual assets. To create a unique asset:

``` csharp
var trevorId = "trevor";
var newUniqueAssetResponse = await client.CreateUniqueAsset(trevorId, horseId);
```

It is also possible to retrieve the details of a unique asset:

``` csharp
var getUniqueAssetResponse = await client.GetUniqueAsset(trevorId, horseId);
```

The amount of a quantifiable asset is held in something equivalent to an account. When the account is created you can also pass an initial balance if required. In the example below this will create an account that holds 10.00 (remember the scale parameter passed in the creation of the asset type).

``` csharp
var accountId = "AccountGBP";
var newQuantifiableAssetResponse = await client.CreateQuantifiableAccount(accountId, gbpId, 1000);
``` 

It is also possible to add extra balance to a quantifiable asset. 

``` csharp
var result = await client.AddBalance(accountId, gbpId, 50);
```

Remebering the scaling, this is effectively creating new assets and adding 0.5 to the balance. In most cases it is likely that a transfer will be used instead to increase the balance of an account.

It is possible to check the balance of an account:

``` csharp
var getQuantifiableAssetResponse = await client.GetQuantifiableAsset(accountId, gbpId);
```

### Transfers

Now we have assets we can transfer them between identities. Let's create a new identity to receive our assets.

``` csharp
var alice = identityBuilder.Create();
var _ = await client.CreateIdentity(alice);
``` 

We next have to create the actual transfer to perform and then send it to the platform. This transfer will be of Trever from the current identity (held in client) to our new identity, Alice.

``` csharp
var transfer = client.CreateOwnershipTransfer(trevorId, horseId, client.Identity.Id, alice.Id);
var response = await _test.Client.TransferAssets(transfer);
```

For the transfer of some quantifiable asset we have to do a little more work. We need an account to transfer the assets into so we will need to set that up first - if the account already exists you won't need to do this step.

``` csharp
var aliceClient = await ClientBuilder.CreateWithExistingIdentity("a url here", alice);
var aliceAccount = aliceClient.CreateUniqueId("AliceAccountGBP");
var _ = await aliceClient.CreateQuantifiableAccount(aliceAccount, gbpId);
```

Now there is an account we can transfer the assets using the original client (as the identity associated with that client currently owns the assets):

``` csharp
var transfer = client.CreateQuantityTransfer(accountId, aliceAccount, gbpId, 100);
var response = await client.TransferAssets(transfer);
```

It is also possible to do multiple transfers in a single call to the platform. In this example we are going to transfer Trevor back and then transfer 0.01 back the other way. As we are removing assets from two identities both identities need to authorise the transfers.

``` csharp

var uniqueTransfer = client.CreateOwnershipTransfer(trevorId, horseId, aliceClient.Identity.Id, client.Identity.Id);
var quantityTransfer = client.CreateQuantityTransfer(accountId, aliceAccount, gbpId, 10);
var body = new TransfersBody(quantityTransfer, uniqueTransfer);
var bodyText = body.Serialize();
var clientAuthorisation = client.GenerateAuthorisation(bodyText);
            
// Pass the body to Alice to sign
var bruceAuthorisation = aliceClient.Client.GenerateAuthorisation(bodyText);

// Alice now returns her authorisations
var transferRequest = new PlatformWriteRequest(body.RequestId, bodyText,
    new[]
    {
        clientAuthorisation,
        aliceAuthorisation
    });

var response = await client.Write(transferRequest);

```

### Claims

Identities, assets and asset types can be "extended" by adding claims to them. A claim is just the hash of some text. The text could be structured or unstructured. The platform takes the hash and the text and checks that the hash is really the hash for the text and then discards the text from that point on. It only stores and deals with the hash after that.

To create a claim (or multiple claims), the call is very similar for each entity:

``` csharp
var identityClaimResponse = await client.CreateIdentityClaims("Some claim"); 
var assetTypeClaimResponse = await client.CreateAssetTypeClaims(horseId, "A claim", "Another claim");
var assetClaimResponse = await client.CreateAssetClaims(horseId, trevorId, "First claim", "Second claim");

```

To retrieve a claim, the calls are also similar. For identities you can get a single claim, the claim of another user or all claims:

``` csharp
var retrievedClaim = await client.GetIdentityClaim("Some Claim");
var retrievedOtherIdentityClaim = await client.GetIdentityClaim(alice.Id, "A claim if it existed");
var retrievedAllClaims = await client.GetIdentityClaims();
var retrievedAllOtherIdentityClaims = await client.GetIdentityClaims(alice.Id);
```

For asset types:

``` csharp
var retrievedAssetTypeClaim = await client.GetAssetTypeClaim(horseId, "A claim");
var retrievedAssetTypeClaims = await client.GetAssetTypeClaims(horseId);
```

For assets:

``` csharp
var retrievedAssetClaim = await client.GetAssetTypeClaim(horseId, trevorId, "First claim");
var retrievedAssetClaims = await client.GetAssetTypeClaims(horseId, trevorId);
```

### Endorsements

The claims that were created can be endorsed by identities - either the same identity or, more likely, other identities. If another identity is involved then both parties need to sign the endorsement being created. This means there is likely to be an out of band communication to get to the point where the endorsement can be submitted.

The first stage is to create the actual endorsement part of the submission. This involves adding the endorsements to the claim. Here Alice is endorsing the identity.

``` csharp
var endorsements = aliceClient.CreateIdentityEndorsements(client.Identity.Id)
                .AddEndorsement("Some claim");
```

The main body of the endorsement request is created next and both parties need to sign it. In this case we can just do it one after another however in the real world this is likely to involve serializing the body and the signing and sending it to the other party to sign and submit.

``` csharp
var body = endorsements.GenerateIdentityEndorsementBody().Serialize();
var aliceHeader = aliceClient.GenerateAuthorisation(body);

// Now the other identity authorizes it
var header = client.GenerateAuthorisation(body);

// Now send them all together
var __ = await client.CreateIdentityClaimsEndorsements(endorsements, endorsements.RequestId, body, header, aliceHeader);

// Now fetch it and verify it            
var endorse = await client.GetIdentityEndorsement(test.Identity.Id, "Some claim", alice.Identity.Id);

// As the client doesn't have access to Alice's private key we need to build something to 
// verify the key using just the Alice's public key (assuming using BouncyCastle)
var key = new BouncyKeyPair(new SerializedKeys().WithPublicKey(alice.Identity.Crypto.Pair.PublicKeyBase64String));
var crypto = new BouncyCrypto(new EcsdaCryptoEngine(), key);
var result = client.VerifyIdentityEndorsement(crypto, client.Identity.Id, "Some claim", endorse.Value.Endorsement);
```

There are equivalent calls for creating endorsements on assets and asset types.

### Delegates

Some situations may require an identity to perform actions on behalf of other identities, for example the employees of a company. To do this we can set an identity to be a delegate and then act on behalf of a delegator.

The first step is to link the delegate and the delegator together. Here we will make our identity a delegate of Alice.

``` csharp
var response = await aliceClient.AddDelegate(client.Identity.Id);
```

Now we have a delegate set up we actually want to use the delegate identity. We take our identity and say that Alice is our delegator.

``` csharp
client.UseDelegator(alice.Id);
```

Any operations we perform now will use the identity but be performed on behalf of Alice - so, permissions permitting, we could create and transfer assets that Alice owns.

To stop using delegation:

``` csharp
clinet.StopUsingDelegator();
```

It is also possible to retrieve all the delegates of a delegator identity.

``` csharp
var response = await aliceClient.GetIdentityDelegates(alice.Id);
```

### Proofs

Any operation that involves a change of state and goes through consensus will generate a proof. It is possible to retrieve a proof using the request id of an operation. Each response will have the request id and also a url to the proof that was generated. To demonstrate this we will create a new identity and then retrieve the proof for that request.

``` csharp
var proofIdentity = identityBuilder.Create();
var proofIdentityResponse = await client.CreateIdentity(proofIdentity);
var proofResponse = await client.GetProof(proofIdentityResponse.Value.RequestId);
```

## Server Hosting

In the case where you need to make calls on behalf of other identities (i.e. you cannot sign things directly as you have no access to the private key) you may want to use different user authorisations and not the ones automatically applied by the simpler API above. In this case there is a PlatformWriteRequest and Write call that will help you achieve this.

Each usage of this follows a pattern:
- Create the body of the message
- Create any authorisations
- Create any authentications (optional)
- Build the request
- Pass the request to the Write method

For example:

``` csharp
var newId = identityBuilder.Create();
var body = new IssueIdentityBody(newId.Id, new Credentials(newId.Crypto.Pair.PublicKeyBase64String, newId.Crypto.ProtocolId));
var bodyText = body.Serialize();
var authorisations = new[] { client.GenerateAuthorisation(bodyText, newId) };
var authentication = client.GenerateAuthentication(authorisations, newId);
var request = new PlatformWriteRequest(body.RequestId, bodyText, authorisations, authentication);
var issueIdentityResponse = await client.Write(request);
``` 

## Solution Structure

- BouncyCastleCrypto - implementation of the required cryptographic calls using the BouncyCastle library. This could be replaced by a different crypto library with the implementation of the ICrypto interface and passing it to the IdentityBuilder.
- Iov42sdk - the main SDK. 
- IntegrationTests - the integration tests for all the API calls that call the core platform. Also includes an example use case for transferring ownership of a car.


