import sys

def load_token(token_path):
    """Obtains the server token"""

    with open(token_path, "r") as token_file:
        return token_file.read().strip()