# Version 1.0.0.9
7th July 2022

- Added permissions to the callas
- Tests for permission calls
- Added missing call for setting claims on a different identity
- Retry handler in tests for GET calls
- Exposed consistency delay as a setting
- Fix for identity in permission calls
- Updated packages including NewtonSoft.json vulnerability

# Version 1.0.0.8
4th July 2022

- Updated .NET core version to 2.1
- Changed to use createIdentity for most identity creation
- Fixed endorsement issue - passing claims in header was causing permission failure
- Simplified the retry logic
- Added client settings to allow more control of timing
- Updated health status API

# Version 1.0.0.7
8th February 2022

- Removed GetRequestStatus call as no longer supported on the core platform


# Version 1.0.0.6
7th February 2022

- Improved the redirect handling to not assume a GET endpoint on the redirect


# Version 1.0.0.5
21st January 2022

- Introduced TransferBuilder to simplify transfer request creation
- Fixed using delegates for "GET" operations
- Updated tests to take into account default permissions for creating assets


# Version 1.0.0.4
24th March 2021

- Made the supported protocols constants so they can be used in switch statements by clients


# Version 1.0.0.3
18th March 2021

- Fixed local package references


# Version 1.0.0.2
12th March 2021

- Changed expected url to not include '/api/v1'. It now expects just the server.


# Version 1.0.0.1
1st March 2021

- Updated to allow the user to verify endorsements
- Added release notes
