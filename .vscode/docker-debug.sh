#!/bin/bash
# The C# extension passes commands like "sh -s" and "/vsdbg/vsdbg --interpreter=vscode"
# as single arguments with spaces. Using sh -c to execute them correctly inside the container.
docker exec -i vitalsync-api-1 sh -c "$1"
