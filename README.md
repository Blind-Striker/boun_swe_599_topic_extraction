# Topic Extraction Using Knowledge Graphs

## Table of Contents

1. [Introduction](#introduction)
2. [Motivation](#motivation)
3. [Prerequisites](#prerequisites)
4. [Getting Started](#getting-started)
5. [Configuration](#configuration)
6. [Running the .NET Application](#running-the-.net-application)
7. [Analyzing with Python Notebook](#analyzing-with-python-notebook)
8. [Under the Hood](#under-the-hood)

## Introduction

This project is a .NET 7 console application that utilizes knowledge graphs for topic extraction from Twitter data, with an emphasis on discussions surrounding OpenAI's ChatGPT. The extracted data is then analyzed using a Python Jupyter notebook for topic and sentiment analysis.

## Motivation

Our digital world is filled with unstructured data that holds untapped insights. The motivation behind this project is to extract meaningful insights from this data. Using Knowledge Graphs, we can uncover not just the explicit information, but also the underlying context that would otherwise remain hidden. Focusing on a popular topic in artificial intelligence - "ChatGPT", we aim to get a deeper understanding of public sentiment and discourse.

## Prerequisites

- You need to have .NET 7.0 SDK installed on your machine. If not, download and install from [here](https://dotnet.microsoft.com/download/dotnet/7.0).
- To run the Python Jupyter notebook, you need Python and Jupyter installed on your machine. If not, follow the installation guides from [here](https://www.python.org/downloads/) and [here](https://jupyter.org/install).

## Getting Started

To get a local copy up and running follow these simple steps:

- Clone the repo
- Navigate to the project directory
- Build the project with following command:

```sh
dotnet build
```

## Configuration

Before running the application, you need to provide the necessary API tokens in the `appsettings.json` file. For instance, you must include your TagMe API GCubeToken and your HuggingFaceClient BearerToken.

Configuration values can also be set via environment variables. For example, instead of setting the HuggingFaceClient's BearerToken in the `appsettings.json` file, you can set it as an environment variable with the name `HuggingFaceClient:BearerToken`.

## Running the .NET Application

You can run the application using the following command:

```sh
dotnet run --project src/TopicExtractionUsingKnowledgeGraphs/TopicExtractionUsingKnowledgeGraphs.csproj
```

The application enriches the tweet data sequentially at each stage (TagMe -> Wikidata -> HuggingFace), saving the enriched tweet data to the specified data folder in the `appsettings.json`. The application can also be resumed from where it left off due to its ability to deserialize previously enriched data from the filesystem.

## Analyzing with Python Notebook

After running the .NET application, you can analyze the enriched data using the Python Jupyter notebook located under report/topic_extraction. This notebook reads the final data, normalizes it, and performs topic and sentiment analysis.

To run the notebook:

Navigate to the `report/topic_extraction` directory
Run the command jupyter notebook to start the Jupyter notebook interface in your browser
Click on the notebook to open it
Follow the instructions in the notebook for analysis
Ensure you have the necessary Python libraries installed. If not, use pip to install the missing libraries:

```sh
pip install -r requirements.txt
```	

Make sure to adjust the paths in the notebook to correctly point to your data if needed.

## Under the Hood

The application is structured around a series of enrichment stages, each leveraging a different API:

- TagMe API for Named Entity Linking (NEL).
- Wikidata Sparql for topic extraction.
- HuggingFace's FinBERT for sentiment analysis.

It reads and processes JSON files downloaded from ScrapeHero and stores the enriched tweets in JSON files in the specified data folder in the `appsettings.json`.