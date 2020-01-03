#!/usr/bin/env bash

version=$1
apiKey="$2"
source="https://api.nuget.org/v3/index.json"
dotnet build
dotnet publish -c Release -o lib
dotnet pack -p:PackageVersion=$version -c Release -o nupkg
dotnet nuget push CAKafka.$1.nupkg -k $apiKey -s $source