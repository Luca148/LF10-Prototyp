#!/bin/sh
set -e

# /app/Data is a persistent volume (see docker-compose.yml).
# On a fresh volume it starts empty, so seed it from the image.
mkdir -p /app/Data

# Rules file: always take the version shipped in the image.
cp /app/Data.seed/Harassment.json /app/Data/Harassment.json

# User store: only seed if it doesn't exist yet, otherwise we'd
# wipe every registered account on each deploy.
[ -f /app/Data/User.json ] || cp /app/Data.seed/User.json /app/Data/User.json

exec "$@"
