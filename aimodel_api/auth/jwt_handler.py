from ..config import settings


def validate_token(token: str):
    return token + ' ' + settings.secret_key
