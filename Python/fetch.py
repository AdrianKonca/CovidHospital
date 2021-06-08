import pandas as pd
import requests
import os.path

DATA_URL = "https://covid.ourworldindata.org/data/owid-covid-data.csv"
RAW_FILE_PATH = "data/raw_file.csv"

def fetch_raw_data_if_missing():
    if os.path.isfile(RAW_FILE_PATH):
        return
    file_request = requests.get(DATA_URL, allow_redirects=True)
    with open(RAW_FILE_PATH, 'wb') as raw_file: 
        raw_file.write(file_request.content)

def clean_raw_file():
    """Cleans raw file leaving only selection of needed columns and removes any countries that do not have hospitalization data"""

    INT_COLUMNS = 'total_cases', 'new_cases', 'total_deaths', 'new_deaths', 'icu_patients', 'hosp_patients'
    NEEDED_COLUMNS = ['iso_code', 'date', *INT_COLUMNS]
    DISCRIMINATORY_COLUMN = 'new_cases'

    raw_data = pd.read_csv(RAW_FILE_PATH)

    processed_data = raw_data[raw_data[DISCRIMINATORY_COLUMN].notnull()][NEEDED_COLUMNS]
    #processed_data.astype({column : int for column in INT_COLUMNS}, copy=False)
    processed_data.to_csv("data/data_preprocessed.csv")
