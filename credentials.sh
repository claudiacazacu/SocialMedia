#!/usr/bin/env bash
# Ruleaza cu: ./credentials.sh
# Afiseaza credentialele efective pentru Adminer si URL public

set -euo pipefail

SUBDOMAIN="${SUBDOMAIN:-${USER:-app}}"
DB_USER="${DB_USER:-$SUBDOMAIN}"
DB_PASSWORD="${DB_PASSWORD:-$SUBDOMAIN-demo-pass-2026}"
DB_NAME="${DB_NAME:-$SUBDOMAIN}"
DB_CONTAINER="student-${USER:-CHANGE_ME}-db"

echo "========================================"
echo "  Credentiale Instagram App"
echo "========================================"
echo ""
echo "  Adminer: https://db.student-dev.ro"
echo "    System:   PostgreSQL"
echo "    Server:   $DB_CONTAINER"
echo "    Username: $DB_USER"
echo "    Password: $DB_PASSWORD"
echo "    Database: $DB_NAME"
echo ""
echo "  App publica: https://${SUBDOMAIN}.student-dev.ro"
echo "  Swagger:     https://${SUBDOMAIN}.student-dev.ro/swagger"
echo "========================================"
