#!/bin/bash
set -e

: "${MSSQL_SA_PASSWORD:?Need to set MSSQL_SA_PASSWORD env var}"

HOST="mssql"
PORT=1433

echo "Waiting for MSSQL at ${HOST}:${PORT} ..."

until /opt/mssql-tools/bin/sqlcmd -S ${HOST} -U sa -P "${MSSQL_SA_PASSWORD}" -Q "SELECT 1" >/dev/null 2>&1; do
  echo "$(date '+%Y-%m-%d %H:%M:%S') - MSSQL not ready, waiting..."
  sleep 2
done

echo "MSSQL is up — running init.sql..."
/opt/mssql-tools/bin/sqlcmd -S ${HOST} -U sa -P "${MSSQL_SA_PASSWORD}" -i /scripts/init.sql

echo "DB init finished."
exit 0