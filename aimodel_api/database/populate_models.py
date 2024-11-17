import json
from sqlalchemy.orm import Session
from .models import Model, ModelStatistics

def add_models_and_statistics(session: Session, json_file_path: str = "./database/initial_data/models.json") -> None:
    try:
        with open(json_file_path, 'r') as f:
            models_data = json.load(f)
    except FileNotFoundError:
        print(f"Error: The file {json_file_path} was not found.")
        return
    except json.JSONDecodeError:
        print(f"Error: The file {json_file_path} is not a valid JSON.")
        return

    models = []
    statistics = []

    for model_data in models_data:
        model = Model(
            name=model_data["name"],
            description=model_data["description"],
            path_to_model=model_data["path_to_model"],
            transformations=model_data["transformations"]
        )
        models.append(model)

    session.add_all(models)
    session.commit()

    for model in models:
        stat = ModelStatistics(
            model_id=model.id,
            total_predictions=0,
            wrong_predictions=0
        )
        statistics.append(stat)

    session.add_all(statistics)
    session.commit()

    print("Models and their statistics have been added from JSON.")
