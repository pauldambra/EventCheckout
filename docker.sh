#! /bin/bash

set -eu

docker pull eventstore/eventstore
docker run --name eventstore \
  -it \
  -p 2113:2113 \
  -p 1113:1113 \
  eventstore/eventstore