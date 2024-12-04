#!/bin/bash

docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.db.yml -f docker-compose.db.override.yml up  --build -d