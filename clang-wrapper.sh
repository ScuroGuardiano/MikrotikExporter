#!/bin/bash

ARGS=()
for arg in "$@"; do
  if [[ "$arg" == "--target=armv7-linux-gnueabihf" ]]; then
    ARGS+=("--target=arm-linux-gnueabihf")
  else
    ARGS+=("$arg")
  fi
done

# Uruchom clang z poprawionymi argumentami
exec /usr/bin/clang-orig "${ARGS[@]}"
