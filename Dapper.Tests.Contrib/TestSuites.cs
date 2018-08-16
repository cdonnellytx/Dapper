using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Xunit;
using Xunit.Sdk;

#if ORACLE
using Oracle.ManagedDataAccess.Client;
#endif

#if SQL_CE
using System.Data.SqlServerCe;
#endif

namespace Dapper.Tests.Contrib
{
    // The test suites here implement TestSuiteBase so that each provider runs
    // the entire set of tests without declarations per method
    // If we want to support a new provider, they need only be added here - not in multiple places

    [XunitTestCaseDiscoverer("Dapper.Tests.SkippableFactDiscoverer", "Dapper.Tests.Contrib")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SkippableFactAttribute : FactAttribute
    {
    }

    public class SqlServerTestSuite : TestSuite
    {
        private const string DbName = "tempdb";
        public static string ConnectionString =>
            IsAppVeyor
                ? @"Server=(local)\SQL2016;Database=tempdb;User ID=sa;Password=Password12!"
                : $"Data Source=.;Initial Catalog={DbName};Integrated Security=True";
        public override IDbConnection GetConnection() => new SqlConnection(ConnectionString);

        static SqlServerTestSuite()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                // ReSharper disable once AccessToDisposedClosure
                Action<string> dropTable = name => connection.Execute($"IF OBJECT_ID('{name}', 'U') IS NOT NULL DROP TABLE [{name}]; ");
                connection.Open();
                dropTable("Stuff");
                connection.Execute("CREATE TABLE Stuff (TheId int IDENTITY(1,1) not null, Name nvarchar(100) not null, Created DateTime null);");
                dropTable("People");
                connection.Execute("CREATE TABLE People (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null);");
                dropTable("Users");
                connection.Execute("CREATE TABLE Users (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null, Age int not null);");
                dropTable("Automobiles");
                connection.Execute("CREATE TABLE Automobiles (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null);");
                dropTable("Results");
                connection.Execute("CREATE TABLE Results (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null, [Order] int not null);");
                dropTable("ObjectX");
                connection.Execute("CREATE TABLE ObjectX (ObjectXId nvarchar(100) not null, Name nvarchar(100) not null);");
                dropTable("ObjectY");
                connection.Execute("CREATE TABLE ObjectY (ObjectYId int not null, Name nvarchar(100) not null);");
                dropTable("ObjectZ");
                connection.Execute("CREATE TABLE ObjectZ (Id int not null, Name nvarchar(100) not null);");
                dropTable("GenericType");
                connection.Execute("CREATE TABLE GenericType (Id nvarchar(100) not null, Name nvarchar(100) not null);");
                dropTable("NullableDates");
                connection.Execute("CREATE TABLE NullableDates (Id int IDENTITY(1,1) not null, DateValue DateTime null);");
            }
        }
    }

    public class MySqlServerTestSuite : TestSuite
    {
        private const string DbName = "DapperContribTests";

        public static string ConnectionString { get; } =
            IsAppVeyor
                ? "Server=localhost;Uid=root;Pwd=Password12!;"
                : "Server=localhost;Uid=test;Pwd=pass;";

        public override IDbConnection GetConnection()
        {
            if (_skip) throw new SkipTestException("Skipping MySQL Tests - no server.");
            return new MySqlConnection(ConnectionString);
        }

        private static readonly bool _skip;

        static MySqlServerTestSuite()
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    // ReSharper disable once AccessToDisposedClosure
                    Action<string> dropTable = name => connection.Execute($"DROP TABLE IF EXISTS `{name}`;");
                    connection.Open();
                    connection.Execute($"DROP DATABASE IF EXISTS {DbName}; CREATE DATABASE {DbName}; USE {DbName};");
                    dropTable("Stuff");
                    connection.Execute("CREATE TABLE Stuff (TheId int not null AUTO_INCREMENT PRIMARY KEY, Name nvarchar(100) not null, Created DateTime null);");
                    dropTable("People");
                    connection.Execute("CREATE TABLE People (Id int not null AUTO_INCREMENT PRIMARY KEY, Name nvarchar(100) not null);");
                    dropTable("Users");
                    connection.Execute("CREATE TABLE Users (Id int not null AUTO_INCREMENT PRIMARY KEY, Name nvarchar(100) not null, Age int not null);");
                    dropTable("Automobiles");
                    connection.Execute("CREATE TABLE Automobiles (Id int not null AUTO_INCREMENT PRIMARY KEY, Name nvarchar(100) not null);");
                    dropTable("Results");
                    connection.Execute("CREATE TABLE Results (Id int not null AUTO_INCREMENT PRIMARY KEY, Name nvarchar(100) not null, `Order` int not null);");
                    dropTable("ObjectX");
                    connection.Execute("CREATE TABLE ObjectX (ObjectXId nvarchar(100) not null, Name nvarchar(100) not null);");
                    dropTable("ObjectY");
                    connection.Execute("CREATE TABLE ObjectY (ObjectYId int not null, Name nvarchar(100) not null);");
                    dropTable("ObjectZ");
                    connection.Execute("CREATE TABLE ObjectZ (Id int not null, Name nvarchar(100) not null);");
                    dropTable("GenericType");
                    connection.Execute("CREATE TABLE GenericType (Id nvarchar(100) not null, Name nvarchar(100) not null);");
                    dropTable("NullableDates");
                    connection.Execute("CREATE TABLE NullableDates (Id int not null AUTO_INCREMENT PRIMARY KEY, DateValue DateTime);");
                }
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Unable to connect"))
                    _skip = true;
                else
                    throw;
            }
        }
    }

    public class SQLiteTestSuite : TestSuite
    {
        private const string FileName = "Test.DB.sqlite";
        public static string ConnectionString => $"Filename=./{FileName};Mode=ReadWriteCreate;";
        public override IDbConnection GetConnection() => new SqliteConnection(ConnectionString);

        static SQLiteTestSuite()
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute("CREATE TABLE Stuff (TheId integer primary key autoincrement not null, Name nvarchar(100) not null, Created DateTime null) ");
                connection.Execute("CREATE TABLE People (Id integer primary key autoincrement not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE Users (Id integer primary key autoincrement not null, Name nvarchar(100) not null, Age int not null) ");
                connection.Execute("CREATE TABLE Automobiles (Id integer primary key autoincrement not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE Results (Id integer primary key autoincrement not null, Name nvarchar(100) not null, [Order] int not null) ");
                connection.Execute("CREATE TABLE ObjectX (ObjectXId nvarchar(100) not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE ObjectY (ObjectYId integer not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE ObjectZ (Id integer not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE GenericType (Id nvarchar(100) not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE NullableDates (Id integer primary key autoincrement not null, DateValue DateTime) ");
            }
        }
    }

#if SQL_CE
    public class SqlCETestSuite : TestSuite
    {
        const string FileName = "Test.DB.sdf";
        public static string ConnectionString => $"Data Source={FileName};";
        public override IDbConnection GetConnection() => new SqlCeConnection(ConnectionString);
            
        static SqlCETestSuite()
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            var engine = new SqlCeEngine(ConnectionString);
            engine.CreateDatabase();
            using (var connection = new SqlCeConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute(@"CREATE TABLE Stuff (TheId int IDENTITY(1,1) not null, Name nvarchar(100) not null, Created DateTime null) ");
                connection.Execute(@"CREATE TABLE People (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE Users (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null, Age int not null) ");
                connection.Execute(@"CREATE TABLE Automobiles (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE Results (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null, [Order] int not null) ");
                connection.Execute(@"CREATE TABLE ObjectX (ObjectXId nvarchar(100) not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE ObjectY (ObjectYId int not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE ObjectZ (Id int not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE GenericType (Id nvarchar(100) not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE NullableDates (Id int IDENTITY(1,1) not null, DateValue DateTime null) ");
            }
            Console.WriteLine("Created database");
        }
    }
#endif


#if ORACLE
    public class OracleTestSuite : TestSuite
    {
        // A usable version of Oracle for testing can be downloaded from 
        // http://www.oracle.com/technetwork/database/database-technologies/express-edition/downloads/index-083047.html


        public static string ConnectionString => "Data Source=MyOracleDB;User Id=DapperContribTests;Password=DapperContribTests;";
        private const string Schema = "DapperContribTests";

        public override IDbConnection GetConnection() => new OracleConnection(ConnectionString);
        
        static OracleTestSuite()
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                connection.Open();

                void dropTable(string table)
                {
                    connection.Execute($@"
                        DECLARE itemExists NUMBER;
                        BEGIN
                            itemExists := 0;

                            SELECT COUNT(*) INTO itemExists
                                FROM ALL_TABLES
                                WHERE OWNER = UPPER('{Schema}') AND TABLE_NAME = UPPER('{table}');
                            
                            IF itemExists > 0 THEN
                                EXECUTE IMMEDIATE 'DROP TABLE {Schema}.{table} CASCADE CONSTRAINTS';
                            END IF;
                        END;
                    ");
                }

                connection.Execute($"ALTER SESSION SET CURRENT_SCHEMA = {Schema}");

                dropTable("Stuff");
                connection.Execute("CREATE TABLE Stuff (TheId integer generated by default on null as identity not null primary key, Name varchar2(100) not null, Created DATE null)");
                dropTable("People");
                connection.Execute("CREATE TABLE People (Id integer generated by default on null as identity not null primary key, Name varchar2(100) not null)");
                dropTable("Users");
                connection.Execute("CREATE TABLE Users (Id integer generated by default on null as identity not null primary key, Name varchar2(100) not null, Age integer not null)");
                dropTable("Automobiles");
                connection.Execute("CREATE TABLE Automobiles (Id integer generated by default on null as identity not null primary key, Name varchar2(100) not null)");
                dropTable("Results");
                connection.Execute("CREATE TABLE Results (Id integer generated by default on null as identity not null primary key, Name varchar2(100) not null, \"ORDER\" integer not null)");
                dropTable("ObjectX");
                connection.Execute("CREATE TABLE ObjectX (ObjectXId varchar2(100) not null, Name varchar2(100) not null)");
                dropTable("ObjectY");
                connection.Execute("CREATE TABLE ObjectY (ObjectYId integer not null, Name varchar2(100) not null)");
                dropTable("ObjectZ");
                connection.Execute("CREATE TABLE ObjectZ (Id integer not null, Name varchar2(100) not null)");
                dropTable("GenericType");
                connection.Execute("CREATE TABLE GenericType (Id varchar2(100) not null, Name varchar2(100) not null)");
                dropTable("NullableDates");
                connection.Execute("CREATE TABLE NullableDates (Id integer generated by default on null as identity not null, DateValue date null)"); // ASSUMPTION: no sub-second precision
            }
        }
    }
#endif
}
