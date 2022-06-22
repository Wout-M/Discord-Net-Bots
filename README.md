# Discord.NET bots

This is a collection of the [Discord](https://discord.com/) bots I've made with [Discord.NET](https://github.com/discord-net/Discord.Net). Feel free to use these bots as a guideline or host your own version of it. 

## About the project
There are currently 4 bots:
- **Discord Bot:** A testing bot that I use to try out new things before moving them over to the actual bots that are being used.
- **ExWi:** A bot for the server for and by the students of the science department of the University of Bern
- **KGB:** A bot that originally was made for logging edited/deleted messages (hence the name). Now it also does some other fun stuff
- **The Wanderers Helper:** A bot for a themed fantasy reading server for a YouTube channel


Some functionalities that one or more bots have:
- Let people choose roles with a button menu
- Log deleted/edited messages
- Ask trivia questions using the [Open Trivia API](https://opentdb.com/)
- Generate fun quotes using the [InspiroBot API](https://inspirobot.me/)
- Check for birthdays of members and automatically make events


## Built With
- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Discord.NET](https://github.com/discord-net/Discord.Net)
- [Quartz.NET](https://www.quartz-scheduler.net/)
- [Docker](https://www.docker.com/) (For hosting the bots on a Raspberry pi)

## Hosting
There are multiple ways to host Discord bots, as can be found in the [Discord.NET documentation](https://discordnet.dev/guides/deployment/deployment.html). I've been hosting them on a simple Raspberry PI, so I'll explain how I've done that.

### Prerequisites
- A Raspberry Pi. I'm using a 3B+ which seems to run 3 bots just fine 
- An SD card with an OS that can run Docker. I'm using an 8GB one with [Raspberry PI OS Lite](https://www.raspberrypi.com/software/operating-systems/) 
- Docker

### Installation
0. This is an optional step, but I've first installed [Portainer](https://www.portainer.io/) to easily maintain the different containers and images remotely
1. Make the following folder structure. Make sure to replace `bot-name` with the name of your bot.
   ```
   {bot-name}
   │
   └───config
   │   │  {This will contain the bot config}
   │
   └───logs
       │   {This will contain the possible error logs}
   ```
2. If you're using one of my bots, clone the branch of your bot in a folder that's also called `bot-name`. Again, replace the name with your own one.
   - `-b`: Clone a specific branch
   - `--single-branch`: Clone only the history of this branch. Future fetches will also only look to this branch
   ```sh
   git clone -b {bot-name} --single-branch https://github.com/Wout-M/Discord-Net-Bots {bot-name}
   ```
   If you're not using one of my bots, just make sure the DLL's and other build files necessary to run the bot are in said folder. The structure should look something like this:
   ```
   {bot-name}
   │
   └───config
   │
   └───logs
   │
   └───{bot-name}
       │   {This contains the DLL's and other files}
   ```

3. In the main folder, create a Dockerfile for building the image
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/aspnet:6.0
   COPY ./{bot-name} ./{bot-name}
   ENTRYPOINT ["dotnet", "{bot-name}.dll"]
   ```
4. Hopefully this step won't be necessary anymore in the future, but for now the bot can't seem to generate a config file by itself from within a container. So, in the `config` folder, create a `config.json` file with the necessary initial configurations. For example for my bots it would be:
   ```json
   {
       "Token": "your-bot-token",
       "OwnerID": 1,
       "Servers": {}
   }
   ```
   The folder structure should now finally look like this:
   ```
   {bot-name}
   │   DockerFile
   │
   └───config
   │   │  config.json
   │
   └───logs
   │
   └───{bot-name}
       │   {DLL's and other files}
   ```
5. Create a Docker image for the bot
    - `-t`: The name of the image
    - `-f`: The Dockerfile location
   ```sh
   docker build -t {bot-name} -f Dockerfile .
   ```
6. Create a container with the newly created image
   - `-d`: Run the container in the background
   - `-v`: Bind a volume on your local environment to a volume on the container. This is used so that the config file and log files are persisted when the container is restarted
   - `--name`: The name of the container
   - `--restart`: Configure the restart policy. I've put it on always so that the container will restart when for example the Pi has been rebooted.
   - `{bot-name}`: The name of the image
   ```sh
   docker run -d -v $(pwd)/config:/config -v $(pwd)/logs:/logs --name {bot-name} --restart always {bot-name}
   ```
7. Enjoy using your bot!    

## Roadmap
- [x] Use Github Actions to build the bots
- [x] Use Docker to host the bots
- [x] Use Quartz to check for birthdays
- [x] Use Discord slash commands
- [ ] Register slash commands upon joining a server
- [ ] Let bot generate config from within a container
