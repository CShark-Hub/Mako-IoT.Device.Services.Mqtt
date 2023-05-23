#  Mako-IoT.Device.Services.Mqtt
ICommunicationService implementation with MQTT using [M2MQTT](https://github.com/nanoframework/nanoFramework.m2mqtt) library.

## How to manually sync fork
- Clone repository and navigate into folder
- From command line execute bellow commands
- **git remote add upstream https://github.com/CShark-Hub/Mako-IoT.Device.Base.git**
- **git fetch upstream**
- **git rebase upstream/main**
- If there are any conflicts, resolve them
  - After run **git rebase --continue**
  - Check for conflicts again
- **git push -f origin main**
