const sql = require('mssql');
require('dotenv').config();

const config = {
  server: process.env.DB_SERVER || 'localhost',
  database: process.env.DB_DATABASE || 'CursosDB',
  options: {
    encrypt: String(process.env.DB_ENCRYPT || 'false') === 'true',
    trustServerCertificate:
      String(process.env.DB_TRUST_SERVER_CERTIFICATE || 'true') === 'true',
    instanceName: process.env.DB_INSTANCE || undefined,
  },
  pool: {
    max: 10,
    min: 0,
    idleTimeoutMillis: 30000,
  },
};

if (process.env.DB_PORT) {
  config.port = Number(process.env.DB_PORT);
}

if (process.env.DB_USER) {
  config.user = process.env.DB_USER;
}

if (process.env.DB_PASSWORD) {
  config.password = process.env.DB_PASSWORD;
}

if (!config.options.instanceName) {
  delete config.options.instanceName;
}

let poolPromise;

function getPool() {
  if (!poolPromise) {
    poolPromise = sql.connect(config).catch((error) => {
      poolPromise = null;
      throw error;
    });
  }

  return poolPromise;
}

module.exports = { sql, getPool };
