from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    app_name: str = ""
    debug: bool = False
    secret_key: str = ""
    
    model_path: str = ""

    model_config = SettingsConfigDict(env_file=".env")
    
settings = Settings()