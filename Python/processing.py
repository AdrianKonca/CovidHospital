import pandas as pd
import matplotlib.pyplot as plt
import datetime
import numpy as np
import json

from _datetime import timedelta
from dataclasses import dataclass
from statistics import mean

#Necessary for silencing warning in interpolation
pd.options.mode.chained_assignment = None

countries = []
CASE_THRESHOLD = 30
FIGURES_PATH = "figures/"
OUTPUT_DATA_PATH = "output_data/"

def clamp(x): 
    return int(max(0, min(x, 255)))

def rgb_to_hex(color):
    return "#{0:02x}{1:02x}{2:02x}".format(clamp(color[0]), clamp(color[1]), clamp(color[2]))

class CovidCountry:
    pass

@dataclass
class Wave:

    cases : dict
    wave_start : pd.Timestamp
    wave_end : pd.Timestamp
    country_name : str
    country_iso : str
    column : str

    @staticmethod
    def merge(waves):
        waves_merged = []
        merges_made = False
        #Using dicionary to avoid duplicates
        waves_to_merge = {}
        # Try to merge overlaping waves until no more can be merged
        for i in range(len(waves) - 1):
            if (waves[i + 1]["date"].min() - waves[i]["date"].max()).days < 1:
                waves_to_merge[i] = waves[i]
                waves_to_merge[i + 1] = waves[i + 1]
            else:
                if (not i in waves_to_merge):
                    waves_merged.append(waves[i])
                if (i == len(waves) - 2) and (not i + 1 in waves_to_merge):
                    waves_merged.append(waves[i + 1])
                #Merge waves if there are any
                if (len(waves_to_merge) != 0):
                    merged_wave = pd.concat(waves_to_merge.values(), ignore_index=True).drop_duplicates().reset_index()
                    waves_merged.append(merged_wave)
                    waves_to_merge = {}
                #Append current wave as it wasn't merged
        if len(waves) == 1:
            waves_merged.append(waves[0])

        # if len(waves) > 0:
        #     print("Country: " + waves[0].country_name.iloc[0])
        # print("Waves: ")
        # for wave in waves:
        #     print("Wave from {0} - {1}".format(wave["date"].min(), wave["date"].max()))
        # print("")
        # print("Waves merged: ")
        # for wave in waves_merged:
        #     print("Wave from {0} - {1}".format(wave["date"].min(), wave["date"].max()))
        #     #print(wave.head())
        # print("")
        # print("")
        return waves_merged

    @staticmethod
    def build_wave_from_dataframe(wave_df, country : CovidCountry, column : str):
        
        wave_df[column] = wave_df[column].interpolate()
        wave = Wave(
            dict(zip(wave_df.date, wave_df[column])),
            wave_df.date.min(),
            wave_df.date.max(),
            country.country_name,
            country.country_iso,
            column,
        )

        return wave
    
    def plot_wave(self, wave_number, y_lim):
        plt.figure(figsize=(16,9))
        
        dates = list(self.cases.keys())
        plt.plot(dates, list(self.cases.values()))
        plt.title(self.country_name)
    
        date_indices = list(np.round((np.linspace(0, len(dates) - 1, 5))).astype(int))
    
        plt.xticks([dates[i] for i in date_indices])

        plt.ylim(y_lim)
        plt.xlim(left = min(dates), right = max(dates))

        filename = "{0}_{1}_{2}".format(self.column, self.country_iso, wave_number)
        plt.savefig(FIGURES_PATH + filename + ".png")
        plt.close()

    def to_dict(self, wave_number):

        data_jsonable = self.__dict__
        data_jsonable['wave_number'] = wave_number
        data_jsonable['average'] = mean(self.cases.values())
        data_jsonable['cases'] = {str(k): v for k, v in self.cases.items()}
        data_jsonable['wave_start'] = str(data_jsonable['wave_start'])
        data_jsonable['wave_end'] = str(data_jsonable['wave_end'])

        return self.__dict__

@dataclass
class CovidCountry:

    data : pd.DataFrame
    country_iso : str
    country_name : str

    #Shared between two methods
    WAVE_START_OFFSET = 14
    WAVE_END_OFFSET = 7

    #Only used for hospital admissions
    WAVE_MINIMUM_DAYS = 14 + WAVE_START_OFFSET + WAVE_END_OFFSET
    MINIMUM_MEAN_NEW_CASES_FOR_WAVE = 2

    #Only used for new cases
    INCREASING_FOR = 4
    DECREASING_FOR = 4 

    DECREASING_ANGLE = -10
    INCREASING_ANGLE = 20

    MINIMUM_MEAN_NEW_CASES_FOR_WAVE = 10

    def build_waves_by_hosp_admission(self, country : CovidCountry):
        waves = []
        wave_start_date = None
        wave_end_date = None
        previous_day = None
        for i, day in self.data.iterrows():
            if i != 0 and day["rolling_hosp_admissions"] > 0 and previous_day["rolling_hosp_admissions"] < 0:
                wave_start_date = day["date"]
                wave_end_date = None
            if i != 0 and day["rolling_hosp_admissions"] < 0 and previous_day["rolling_hosp_admissions"] > 0:
                wave_end_date = day["date"]
            if wave_start_date and wave_end_date:
                if ((wave_end_date - wave_start_date).days > self.WAVE_MINIMUM_DAYS):

                    wave = self.data[(self.data['date'] > wave_start_date) & (self.data['date'] <= wave_end_date)]
                    wave_extended = self.data[
                        (self.data['date'] > wave_start_date - timedelta(days=self.WAVE_START_OFFSET)) & 
                        (self.data['date'] <= wave_end_date + timedelta(days=self.WAVE_END_OFFSET))
                    ]
                    if (wave["rolling_hosp_admissions"].mean() > 2):
                        waves.append(wave_extended)
                wave_start_date = None
                wave_end_date = None

            previous_day = day

        self.waves_admissions = [Wave.build_wave_from_dataframe(wave, self, 'rolling_hosp_admissions') for wave in Wave.merge(waves)]

        return self.waves_admissions

    def build_waves_by_new_cases(self, country : CovidCountry):

        waves = []
        wave_start_date = None
        wave_start_count = 0
        wave_end_date = None
        wave_end_count = 0
        previous_day = None
        # plt.figure(figsize=(16,9))
        for i, day in self.data.iterrows():
            if (i != 0) and pd.notnull(day["rolling_new_cases"]):
                #as we are going day by day the denominator is constant and is equal to 1
                angle = np.rad2deg(np.arctan2(day["rolling_new_cases"] - previous_day["rolling_new_cases"], 1))
                #color = None
                
                if angle < self.DECREASING_ANGLE:
                    if wave_start_date and wave_end_date is None and wave_end_count == 0:
                        wave_end_date = day["date"]
                    if wave_start_date and wave_end_date and wave_end_count < self.DECREASING_FOR:
                        wave_end_count += 1
                    if wave_start_count < self.INCREASING_FOR:
                        wave_start_count = 0
                        wave_start_date = None
                elif angle > self.INCREASING_ANGLE:
                    if wave_start_date is None and wave_start_count == 0:
                        wave_start_date = day["date"]
                    if wave_start_date and wave_start_count < self.INCREASING_FOR:
                        wave_start_count += 1
                    if wave_end_count < self.DECREASING_FOR:
                        wave_end_count = 0
                        wave_end_date = None

                if (wave_start_count == self.INCREASING_FOR and wave_end_count == self.DECREASING_FOR):
                    mask_wave = (
                        (self.data['date'] > wave_start_date) & 
                        (self.data['date'] <= wave_end_date)
                    )
                    mean_case_count_during_wave = self.data[mask_wave]["rolling_new_cases"].mean()
                    if mean_case_count_during_wave > self.MINIMUM_MEAN_NEW_CASES_FOR_WAVE:
                        mask_wave_extended = (
                            (self.data['date'] > wave_start_date - timedelta(days=self.WAVE_START_OFFSET)) & 
                            (self.data['date'] <= wave_end_date + timedelta(days=self.WAVE_END_OFFSET))
                        )
                        waves.append(self.data[mask_wave_extended & self.data['rolling_new_cases'].notna()])
                    wave_start_count = 0
                    wave_start_date = None
                    wave_end_count = 0
                    wave_end_date = None

            if len(self.data.index) == (i + 1) and wave_start_count == self.INCREASING_FOR and wave_end_count < self.DECREASING_FOR:
                wave_end_date = day["date"]
                mask_wave = (
                    (self.data['date'] > wave_start_date) & 
                    (self.data['date'] <= wave_end_date)
                )
                mean_case_count_during_wave = self.data[mask_wave]["rolling_new_cases"].mean()
                if mean_case_count_during_wave > self.MINIMUM_MEAN_NEW_CASES_FOR_WAVE:
                    mask_wave_extended = (
                        (self.data['date'] > wave_start_date - timedelta(days=self.WAVE_START_OFFSET)) & 
                        (self.data['date'] <= wave_end_date + timedelta(days=self.WAVE_END_OFFSET))
                    )
                    waves.append(self.data[mask_wave_extended & self.data['rolling_new_cases'].notna()])

            previous_day = day
        merged_waves = Wave.merge(waves)
        filtered_waves = []

        for wave in merged_waves:
            mean_case_count_during_wave = wave["rolling_new_cases"].mean()
            if mean_case_count_during_wave > self.MINIMUM_MEAN_NEW_CASES_FOR_WAVE:
                filtered_waves.append(
                    Wave.build_wave_from_dataframe(wave, country, 'rolling_new_cases')
                )

        self.waves_cases = filtered_waves

        # plt.title(country_iso)

        # plt.savefig(FIGURES_PATH + country_iso + ".png")
        # plt.close()
        return filtered_waves
        

def normalize():
    # https://data.worldbank.org/indicator/SP.POP.TOTL
    COLUMNS_TO_NORMALIZE = ['total_cases', 'new_cases', 'total_deaths', 'new_deaths', 'icu_patients', 'hosp_patients']

    population_data = pd.read_csv("data/population_data_2019.csv")
    covid_data = pd.read_csv("data/data_preprocessed.csv")
    covid_data = pd.merge(covid_data, population_data, on=["iso_code"], how="left")
    for column in COLUMNS_TO_NORMALIZE:
        covid_data[column] = covid_data[column] / covid_data["population"] * 1000000
    return covid_data

def process_file(covid_data):
    
    covid_data["date"] = pd.to_datetime(covid_data["date"])
    #covid_data = covid_data[covid_data["date"] > pd.Timestamp(2020, 4, 1)]
    wave_begin = None
    wave_stop = None
    countries = []
    max_cases = 0
    max_admissions = 0
    min_admissions = 0
    for country_iso, country_data in covid_data.groupby("iso_code"):
        country_data = country_data.reset_index()
        country_data["rolling_hosp_patients"] = country_data["hosp_patients"].rolling(7).mean()
        country_data["rolling_hosp_admissions"] = country_data["rolling_hosp_patients"].diff().rolling(7).mean()
        
        country_data["rolling_new_cases"] = country_data["new_cases"].clip(0).rolling(28).mean()

        country = CovidCountry(country_data, country_iso, country_data.country_name.iloc[0])
        #if country_iso in ["USA", "FRA", "GBR", "ITA", "ESP"] :
        #    continue
        #if country_iso not in ["POL"] :
        #   continue
        #min_date = country_data["date"].min()
        #max_date = country_data["date"].max()
        
        #plt.plot(country_data["date"], country_data["rolling_hosp_admissions"])
        #m = country_data["rolling_hosp_admissions"].mean()

        #plt.plot([min_date, max_date], [m, m])
        #plt.plot(country_data["date"], country_data["rolling_hosp_patients"])

        # waves = build_waves_by_hosp_admission(country_data)
        # for wave in waves:
        #     plt.plot(wave["date"], wave["rolling_hosp_admissions"])
        #     countries.append(country_iso)

        if (country_data["population"].head(1) > 10**7).any() and country_data["rolling_new_cases"].mean() > 10:
            country.build_waves_by_hosp_admission(country)
            country.build_waves_by_new_cases(country)
        if hasattr(country, 'waves_cases'):
            for wave in country.waves_cases:
                max_cases = max(max_cases, max(list(wave.cases.values())))
        if hasattr(country, 'waves_admissions'):
            for wave in country.waves_admissions:
                if (max_admissions < max(list(wave.cases.values()))):
                    print(max(list(wave.cases.values())))
                max_admissions = max(max_admissions, max(list(wave.cases.values())))
                min_admissions = min(min_admissions, min(list(wave.cases.values())))

        countries.append(country)

    country_dictionaries_cases = []
    country_dictionaries_admissions = []
    for country in countries:
        if hasattr(country, 'waves_cases'):
            country.waves_cases.sort(key=lambda w: w.wave_start)
            for i, wave in enumerate(country.waves_cases):
                wave.plot_wave(i + 1, [0, (max_cases // 100 + 1) * 100])
                country_dictionaries_cases.append(wave.to_dict(i))
        
        if hasattr(country, 'waves_admissions'):
            country.waves_admissions.sort(key=lambda w: w.wave_start)
            for i, wave in enumerate(country.waves_admissions):
                wave.plot_wave(i + 1, [(min_admissions // 10 + -1) * 10, (max_admissions // 10 + 1) * 10])
                country_dictionaries_admissions.append(wave.to_dict(i))

    with open(OUTPUT_DATA_PATH + 'cases_data_by_country.json', 'w') as outfile:
        json.dump(country_dictionaries_cases, outfile, indent=2)
    with open(OUTPUT_DATA_PATH + 'admissions_data_by_country.json', 'w') as outfile:
        json.dump(country_dictionaries_admissions, outfile, indent=2)

    #plt.legend(countries)
    #plt.gca().set_yscale('log')
    #plt.xticks(list(range(0, 451, 30)))
    return covid_data

covid_data = normalize()
process_file(covid_data)