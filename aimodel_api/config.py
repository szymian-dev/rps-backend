from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    app_name: str = ""
    debug: bool = False
    secret_key: str = ""
    valid_issuer: str = ""
    
    ai_models_folder: str = ""
    unet_model_name: str = ""
    
    skip_auth: bool = False

    model_config = SettingsConfigDict(env_file=".env")
    
settings = Settings()