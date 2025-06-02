# Użyj oficjalnego obrazu Pythona jako bazowego
FROM python:3.12-slim

WORKDIR /app

RUN apt-get update && apt-get install ffmpeg libsm6 libxext6 libglib2.0-0 libxrender1 -y
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt
COPY . .

EXPOSE 8000

CMD ["fastapi", "run", "main.py", "--port", "8000"]