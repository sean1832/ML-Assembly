from git import Repo
from datetime import datetime
import pandas as pd
import os

# Change to parent directory
os.chdir('..')

# Specify the repository
local_repo = Repo(r"E:\00-Cloud\OneDrive\Project\Reserach-Assistance-ML-Assembly")
web_repo = "https://github.com/sean1832/Research-Assistance-ML-Assembly"
branch = 'main'


def GetCommitData(start_date: datetime, end_date: datetime):
    # Iterate over the commits
    data = []
    for commit in local_repo.iter_commits(branch):
        # Get commit date
        commit_date = datetime.fromtimestamp(commit.committed_date)

        # Check if commit_date is in the range
        if start_date <= commit_date <= end_date:
            data.append({
                'SHA': commit.hexsha,
                'Date': commit_date,
                'Message': commit.message,
                'GitHub URL': f"{web_repo}/commit/{commit.hexsha}"
            })
    return data


def main():
    # Prompt the user for the start date and end date
    start_date_str = input("Please enter the start date (YYYYMMDD): ")
    end_date_str = input("Please enter the end date (YYYYMMDD): ")

    # Convert the entered strings into datetime objects
    start_date = datetime.strptime(start_date_str, '%Y%m%d')
    end_date = datetime.strptime(end_date_str, '%Y%m%d')

    # Get the commit data
    data = GetCommitData(start_date, end_date)

    # Create a DataFrame
    df = pd.DataFrame(data)
    print(df)

    # Ask user if they want to export the data to an Excel file
    export = input("Would you like to export the data to a file? (y/n): ")
    if export == 'y':
        file_dir = "Data"
        file_name = "commit_data.xlsx"
        export_path = f"{file_dir}/{file_name}"
        # if file_dir not exists, create it
        if not os.path.exists(file_dir):
            os.makedirs(file_dir)

        with pd.ExcelWriter(f"{file_dir}/{file_name}", engine='xlsxwriter') as writer:
            df.to_excel(writer, index=False, sheet_name='CommitData')

            # get xlsxwriter objects
            workbook = writer.book
            worksheet = writer.sheets['CommitData']

            # apply link format
            link_format = workbook.add_format({'font_color': 'blue', 'underline': 1})

            # iterate over the urls column and apply the link format
            for row, url in enumerate(df['GitHub URL'], start=1):
                worksheet.write_url(f'D{row + 1}', url, link_format)

        print(f"Data exported to {file_name}!")


if __name__ == "__main__":
    main()
