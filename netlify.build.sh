﻿#!/usr/bin/env bash
set -e

# Change into a temporary directory so we can download + install .NET
pushd /tmp

# Download Microsoft install script
wget https://dot.net/v1/dotnet-install.sh

# Make the script executable
chmod +x ./dotnet-install.sh

# Install .NET 8 (modify channel if needed in the future)
./dotnet-install.sh --channel 8.0

# Return to project directory
popd

# Now we can publish (since we've installed .NET in this environment)
dotnet publish -c Release -o release