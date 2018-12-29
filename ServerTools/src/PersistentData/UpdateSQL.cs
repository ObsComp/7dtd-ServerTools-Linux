using System.Data;

namespace ServerTools
{
    public class UpdateSQL
    {
        public static void Exec(int _version)
        {
            if (_version == 1)
            {
                MySqlDatabase.FastQuery("ALTER TABLE Players ADD return VARCHAR(50) DEFAULT 'false'; ALTER TABLE Players ADD eventRespawn VARCHAR(50) DEFAULT 'false'; ALTER TABLE Players ADD eventSpawn VARCHAR(50) DEFAULT 'false';");
                SQL.FastQuery("UPDATE Config SET sql_version = 2 WHERE sql_version = 1");
            }
            if (_version == 2)
            {
                MySqlDatabase.FastQuery("ALTER TABLE EventSpawns DROP eventSpawn; ALTER TABLE EventSpawns DROP eventRespawn;");
                MySqlDatabase.FastQuery("ALTER TABLE EventSpawns ADD eventSpawn VARCHAR(50); ALTER TABLE EventSpawns ADD eventRespawn VARCHAR(50);");
                SQL.FastQuery("UPDATE Config SET sql_version = 3 WHERE sql_version = 2");
            }
            if (_version == 3)
            {
                MySqlDatabase.FastQuery("ALTER TABLE Players ADD newSpawn VARCHAR(50) DEFAULT 'false'; ALTER TABLE Auction ADD sellDate VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM'; ALTER TABLE Players ADD messageCount INT DEFAULT 0; ALTER TABLE Players ADD messageTime VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM';");
                SQL.FastQuery("UPDATE Config SET sql_version = 4 WHERE sql_version = 3");
            }
            if (_version == 4)
            {
                MySqlDatabase.FastQuery("CREATE TABLE IF NOT EXISTS Vehicles (steamid INTEGER PRIMARY KEY, bikeId INT DEFAULT 0, lastBike VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', miniBikeId INT DEFAULT 0, lastMiniBike VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', motorBikeId INT DEFAULT 0, lastMotorBike VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', jeepId INT DEFAULT 0, lastJeep VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', gyroId INT DEFAULT 0, lastGyro VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM')");
                SQL.FastQuery("UPDATE Config SET sql_version = 5 WHERE sql_version = 4");
            }
            if (_version == 5)
            {
                MySqlDatabase.FastQuery("ALTER TABLE Players ADD lastVoteWeekly VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM'; ALTER TABLE Players ADD weeklyVoteCount INT DEFAULT 0;");
                SQL.FastQuery("UPDATE Config SET sql_version = 6 WHERE sql_version = 5");
            }
            CheckVersion();
        }

        private static void CheckVersion()
        {
            DataTable _result = SQL.TQuery("SELECT sql_version FROM Config");
            int _version;
            int.TryParse(_result.Rows[0].ItemArray.GetValue(0).ToString(), out _version);
            _result.Dispose();
            if (_version != SQL.Sql_version)
            {
                Exec(_version);
            }
            else
            {
                LoadProcess.Load(3);
            }
        }
    }
}
