# Rock Paper Scissors - AI, backend
### A web application - the "Rock, Paper, Scissors" game - using an artificial intelligence model to recognize gestures in photos - backend layer.
Celem pracy jest opracowanie backendu dla interaktywnej gry online "Papier, Kamień, Nożyce", z wykorzystaniem sztucznej inteligencji do rozpoznawania gestów przesyłanych przez użytkowników. Praca obejmuje zaprojektowanie i implementację serwera, który będzie zarządzał komunikacją między frontendem, użytkownikami oraz modelem AI. Backend będzie odpowiedzialny za autoryzację użytkowników, przetwarzanie zdjęć gestów przesyłanych przez graczy, przekazywanie ich do modelu AI oraz zwracanie wyników rozgrywki. Ważnym aspektem projektu będzie integracja modelu sztucznej inteligencji, który będzie analizował gesty pozwalając wyłonić zwycięzcę każdej rundy.

## Main API
### Requirements
- .NET 8 (https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- PostgreSQL (https://www.postgresql.org/download/)
#### Optional
- PgAdmin (https://www.pgadmin.org/download/)
- Rider (https://www.jetbrains.com/rider/download/) or Visual Studio (https://visualstudio.microsoft.com/pl/downloads/)
### Installation guide

#### Database
The project comes with a preconfigured `appsettings.Development.json` file, which contains the connection string to the database. You can change it to your own database connection string. The database will be created automatically when you run the application for the first time.

By default the connection string is set to:
```json
"ConnectionStrings": {
    "RpsDatabaseConnection": "Host=localhost;Port=5432;Database=RPS;Username=RPSuser;Password=rps1234;"
  },
```
To be able to run the default configuration, you need to create a database user with the following credentials:
- Username: **RPSuser**
- Password: **rps1234**
The user needs to have permissions to create databases.
#### Running the application
From `main_api/RpsApi` run the following commands:
To restore dependencies, this will download all the necessary packages if you don't have them already in the `{your_user_folder}/.nuget/packages` folder:
```bash
dotnet restore
``` 
To run the application:
```bash
dotnet run
``` 

## AI Model API
### Requirements
- Python, recommended version `3.12.2` (https://www.python.org/downloads/)
- Pip (https://pip.pypa.io/en/stable/installation/)
#### Optional
- Anaconda, virtual Python environment (https://www.anaconda.com/products/individual)
### Installation guide
To install the required packages, run the following command from the `aimodel_api` directory:
```bash
pip install -r requirements.txt
```
#### Running the application
To run the application, execute the following command from the `aimodel_api` directory:
```bash
fastapi run main.py --port 8000
```
Port `8000` is the default port, you can change it to any other available port. After changing the port, you need to update the AI Model API Url in the `main_api/RpsApi/appsettings.Development.json` file to the new port.
```json
"AiModelApiSettings": {
    "Url": "http://127.0.0.1:8000/api/v1/",
    "Endpoint": "predictions"
  },
```
### Configuration
The AI Model API uses the `.env` file to configure the project. The file is located in the `aimodel_api` directory. 

The crucial configuration is the path to the AI Model file. In order to be able to use `predictions` you need to have a pretrained AI Model file (.h5) on the disk. To configure the path to the model, you need to update the `AI_MODEL_PATH` variable in the `.env` file.

Example:
```env
AI_MODEL_PATH=../model/ai_model.h5
```