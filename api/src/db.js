const sql = require('mssql/msnodesqlv8');
require('dotenv').config();

const config = {
  connectionString:
    'Driver={ODBC Driver 18 for SQL Server};' +
    'Server=(localdb)\\MSSQLLocalDB;' +
    'Database=CursosDB;' +
    'Trusted_Connection=Yes;' +
    'TrustServerCertificate=Yes;',

  options: {
    trustedConnection: true,
    trustServerCertificate: true,
    encrypt: false,
  },

  pool: {
    max: 10,
    min: 0,
    idleTimeoutMillis: 30000,
  },
};

let poolPromise;

function getPool() {
  if (!poolPromise) {
    poolPromise = sql.connect(config);
  }

  return poolPromise;
}

module.exports = {
  sql,
  getPool,
};