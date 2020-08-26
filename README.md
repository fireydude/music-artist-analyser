# AireLogic Tech Test
 > ## Interact with APIs
 > ### Goal
 > Produce a program which, when given the name of an artist, will produce the average number of words in their songs.

The original test can be found in `Interact with APIs.pdf` at the route of this repository.
 ## Summary
 A console application was produced which uses two APIs to first build a list of tracks for a given artist, then retrieve the lyrics for each track.  A calculation is then used to get the average number of words per song for the artist.

 ## Development Environment
 VS Code was used to debug the project.  Please install dotnet core 3.1 and the C# extension if  you wish to load the project from VS Code.  A solution file has been included so Visual Studio 2019 could also be used to debug the project.

### External Dependencies
The following APIs were used to return data about the Music and Lyrics
1. MusixMatch - used to find the tracks for an artist
2. ApiSeeds - used to get the lyrics for the tracks

The application is structured so that these APIs could be replaced without modifying the core of the application.

### Application Settings
Settings are loaded from `./AireLogic.ArtistData/appsettings.json`.

## Run the from the command line

1. Navigate to the `./AireLogic.ArtistData`, where the console application is located. Then build the project: `dotnet build`
2. Run the application, providing the name of an artist: `dotnet run "the beetles"`

## Design Features
The code has been designed with good practices in mind.
1. Dependency Injection has been used to allow for separation of concerns and to make the application extensible
2. Serilog has been used to provide more logging options.  Logs has been sent directly to the console however they could be sent to a database or another sink.
3. Application settings are used to store configurable values such as API keys

Other stretch goals were not implemented.