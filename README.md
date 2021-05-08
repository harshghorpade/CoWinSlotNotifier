# CoWinSlotNotifier
Micro-service used to get email (or SMS) notifications when vaccine slots opens up for booking from CoWin app India. Program works based on specified location (by pincode) or districts over India.

## Prerequisite
1] Need AWS account. Amazon has provide free evaluation period, so if you don't have AWS account, then you can create it without any charges.

2] Create one SNS Topic in any region (say US-East-1) and create one email subscription on that topic with your preferred email id.

3] You need to confirm the SNS email subscription, by checking your email and clicking on the link sent by AWS on creating subscription on topic in step 2.

4] Visual Studio 2019 with .NET Core 3.1 installed.

5] AWS Toolkit for Visual Studio 2019 (optional).

## Steps to setup for local debugging
1] Clone the repository and open the solution file in Visual Studio 2019 or latest.

2] For local debugging need to set few environment variables, those can be set under /properties/launchSettings.json. (ENV_SNS_TOPIC: SNS Topic Urn created in Prerequisite step 2, ENV_AWS_REGION: AWS Region under which SNS Topic has created, for example "us-east-1").

3] Also configure your AWS aws_access_key_id and aws_secret_access_key in "credentials" file at "C:\Users\<Your system user name>\.aws" folder. Alternative to this step is to login to your AWS account from Visual Studio using AWS Toolkit window, this will automatically create the mentioned file in required format.

4] Open "CoWinListener.cs" file and modify the pincodes or district id according to your need in method name "PreparePincodeAndDistrictData" and rebuild the project in Visual Studio. Program should compile/build successfully.

5] Now we are all set to start local debugging, start the debugger from Visual Studio (by hitting F5 key).

6] This will start the console window with message "CoWin Slot Notifier service has started, you should get the email notifications when vaccination slots opens up for booking"
