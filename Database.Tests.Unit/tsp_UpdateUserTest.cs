using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
using Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Database.Tests.Unit
{
    [TestClass()]
    public class tsp_UpdateUserTest : SqlDatabaseTestClass
    {

        public tsp_UpdateUserTest()
        {
            InitializeComponent();
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            base.InitializeTest();
        }
        [TestCleanup()]
        public void TestCleanup()
        {
            base.CleanupTest();
        }

        #region Designer support code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction dbo_tsp_UpdateUserTest_NewName_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(tsp_UpdateUserTest));
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction testInitializeAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction testCleanupAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition rowCountCondition1;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition1;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction dbo_tsp_UpdateUserTest_NewAll_TestAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition rowCountCondition2;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition2;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition3;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition4;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition5;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition6;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition7;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition8;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition9;
            this.dbo_tsp_UpdateUserTest_NewNameData = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            this.dbo_tsp_UpdateUserTest_NewAllData = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            dbo_tsp_UpdateUserTest_NewName_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            testInitializeAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            testCleanupAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            rowCountCondition1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition();
            scalarValueCondition1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            dbo_tsp_UpdateUserTest_NewAll_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            rowCountCondition2 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition();
            scalarValueCondition2 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            scalarValueCondition3 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            scalarValueCondition4 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            scalarValueCondition5 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            scalarValueCondition6 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            scalarValueCondition7 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            scalarValueCondition8 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            scalarValueCondition9 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            // 
            // dbo_tsp_UpdateUserTest_NewNameData
            // 
            this.dbo_tsp_UpdateUserTest_NewNameData.PosttestAction = null;
            this.dbo_tsp_UpdateUserTest_NewNameData.PretestAction = null;
            this.dbo_tsp_UpdateUserTest_NewNameData.TestAction = dbo_tsp_UpdateUserTest_NewName_TestAction;
            // 
            // dbo_tsp_UpdateUserTest_NewName_TestAction
            // 
            dbo_tsp_UpdateUserTest_NewName_TestAction.Conditions.Add(rowCountCondition1);
            dbo_tsp_UpdateUserTest_NewName_TestAction.Conditions.Add(scalarValueCondition1);
            resources.ApplyResources(dbo_tsp_UpdateUserTest_NewName_TestAction, "dbo_tsp_UpdateUserTest_NewName_TestAction");
            // 
            // testInitializeAction
            // 
            resources.ApplyResources(testInitializeAction, "testInitializeAction");
            // 
            // testCleanupAction
            // 
            resources.ApplyResources(testCleanupAction, "testCleanupAction");
            // 
            // rowCountCondition1
            // 
            rowCountCondition1.Enabled = true;
            rowCountCondition1.Name = "rowCountCondition1";
            rowCountCondition1.ResultSet = 1;
            rowCountCondition1.RowCount = 1;
            // 
            // scalarValueCondition1
            // 
            scalarValueCondition1.ColumnNumber = 3;
            scalarValueCondition1.Enabled = true;
            scalarValueCondition1.ExpectedValue = "New Name";
            scalarValueCondition1.Name = "scalarValueCondition1";
            scalarValueCondition1.NullExpected = false;
            scalarValueCondition1.ResultSet = 1;
            scalarValueCondition1.RowNumber = 1;
            // 
            // dbo_tsp_UpdateUserTest_NewAllData
            // 
            this.dbo_tsp_UpdateUserTest_NewAllData.PosttestAction = null;
            this.dbo_tsp_UpdateUserTest_NewAllData.PretestAction = null;
            this.dbo_tsp_UpdateUserTest_NewAllData.TestAction = dbo_tsp_UpdateUserTest_NewAll_TestAction;
            // 
            // dbo_tsp_UpdateUserTest_NewAll_TestAction
            // 
            dbo_tsp_UpdateUserTest_NewAll_TestAction.Conditions.Add(rowCountCondition2);
            dbo_tsp_UpdateUserTest_NewAll_TestAction.Conditions.Add(scalarValueCondition2);
            dbo_tsp_UpdateUserTest_NewAll_TestAction.Conditions.Add(scalarValueCondition3);
            dbo_tsp_UpdateUserTest_NewAll_TestAction.Conditions.Add(scalarValueCondition4);
            dbo_tsp_UpdateUserTest_NewAll_TestAction.Conditions.Add(scalarValueCondition5);
            dbo_tsp_UpdateUserTest_NewAll_TestAction.Conditions.Add(scalarValueCondition6);
            dbo_tsp_UpdateUserTest_NewAll_TestAction.Conditions.Add(scalarValueCondition7);
            dbo_tsp_UpdateUserTest_NewAll_TestAction.Conditions.Add(scalarValueCondition8);
            dbo_tsp_UpdateUserTest_NewAll_TestAction.Conditions.Add(scalarValueCondition9);
            resources.ApplyResources(dbo_tsp_UpdateUserTest_NewAll_TestAction, "dbo_tsp_UpdateUserTest_NewAll_TestAction");
            // 
            // rowCountCondition2
            // 
            rowCountCondition2.Enabled = true;
            rowCountCondition2.Name = "rowCountCondition2";
            rowCountCondition2.ResultSet = 1;
            rowCountCondition2.RowCount = 1;
            // 
            // scalarValueCondition2
            // 
            scalarValueCondition2.ColumnNumber = 1;
            scalarValueCondition2.Enabled = true;
            scalarValueCondition2.ExpectedValue = "1";
            scalarValueCondition2.Name = "scalarValueCondition2";
            scalarValueCondition2.NullExpected = false;
            scalarValueCondition2.ResultSet = 1;
            scalarValueCondition2.RowNumber = 1;
            // 
            // scalarValueCondition3
            // 
            scalarValueCondition3.ColumnNumber = 2;
            scalarValueCondition3.Enabled = true;
            scalarValueCondition3.ExpectedValue = "new@example.com";
            scalarValueCondition3.Name = "scalarValueCondition3";
            scalarValueCondition3.NullExpected = false;
            scalarValueCondition3.ResultSet = 1;
            scalarValueCondition3.RowNumber = 1;
            // 
            // scalarValueCondition4
            // 
            scalarValueCondition4.ColumnNumber = 3;
            scalarValueCondition4.Enabled = true;
            scalarValueCondition4.ExpectedValue = "New Name";
            scalarValueCondition4.Name = "scalarValueCondition4";
            scalarValueCondition4.NullExpected = false;
            scalarValueCondition4.ResultSet = 1;
            scalarValueCondition4.RowNumber = 1;
            // 
            // scalarValueCondition5
            // 
            scalarValueCondition5.ColumnNumber = 4;
            scalarValueCondition5.Enabled = true;
            scalarValueCondition5.ExpectedValue = "NewPassword";
            scalarValueCondition5.Name = "scalarValueCondition5";
            scalarValueCondition5.NullExpected = false;
            scalarValueCondition5.ResultSet = 1;
            scalarValueCondition5.RowNumber = 1;
            // 
            // scalarValueCondition6
            // 
            scalarValueCondition6.ColumnNumber = 5;
            scalarValueCondition6.Enabled = true;
            scalarValueCondition6.ExpectedValue = "2";
            scalarValueCondition6.Name = "scalarValueCondition6";
            scalarValueCondition6.NullExpected = false;
            scalarValueCondition6.ResultSet = 1;
            scalarValueCondition6.RowNumber = 1;
            // 
            // scalarValueCondition7
            // 
            scalarValueCondition7.ColumnNumber = 6;
            scalarValueCondition7.Enabled = true;
            scalarValueCondition7.ExpectedValue = "1234567890123456";
            scalarValueCondition7.Name = "scalarValueCondition7";
            scalarValueCondition7.NullExpected = false;
            scalarValueCondition7.ResultSet = 1;
            scalarValueCondition7.RowNumber = 1;
            // 
            // scalarValueCondition8
            // 
            scalarValueCondition8.ColumnNumber = 7;
            scalarValueCondition8.Enabled = true;
            scalarValueCondition8.ExpectedValue = "1";
            scalarValueCondition8.Name = "scalarValueCondition8";
            scalarValueCondition8.NullExpected = false;
            scalarValueCondition8.ResultSet = 1;
            scalarValueCondition8.RowNumber = 1;
            // 
            // scalarValueCondition9
            // 
            scalarValueCondition9.ColumnNumber = 8;
            scalarValueCondition9.Enabled = true;
            scalarValueCondition9.ExpectedValue = "True";
            scalarValueCondition9.Name = "scalarValueCondition9";
            scalarValueCondition9.NullExpected = false;
            scalarValueCondition9.ResultSet = 1;
            scalarValueCondition9.RowNumber = 1;
            // 
            // tsp_UpdateUserTest
            // 
            this.TestCleanupAction = testCleanupAction;
            this.TestInitializeAction = testInitializeAction;
        }

        #endregion


        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        #endregion

        [TestMethod()]
        public void dbo_tsp_UpdateUserTest_NewName()
        {
            SqlDatabaseTestActions testActions = this.dbo_tsp_UpdateUserTest_NewNameData;
            // Execute the pre-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PretestAction != null), "Executing pre-test script...");
            SqlExecutionResult[] pretestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PretestAction);
            try
            {
                // Execute the test script
                // 
                System.Diagnostics.Trace.WriteLineIf((testActions.TestAction != null), "Executing test script...");
                SqlExecutionResult[] testResults = TestService.Execute(this.ExecutionContext, this.PrivilegedContext, testActions.TestAction);
            }
            finally
            {
                // Execute the post-test script
                // 
                System.Diagnostics.Trace.WriteLineIf((testActions.PosttestAction != null), "Executing post-test script...");
                SqlExecutionResult[] posttestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PosttestAction);
            }
        }
        [TestMethod()]
        public void dbo_tsp_UpdateUserTest_NewAll()
        {
            SqlDatabaseTestActions testActions = this.dbo_tsp_UpdateUserTest_NewAllData;
            // Execute the pre-test script
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PretestAction != null), "Executing pre-test script...");
            SqlExecutionResult[] pretestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PretestAction);
            try
            {
                // Execute the test script
                // 
                System.Diagnostics.Trace.WriteLineIf((testActions.TestAction != null), "Executing test script...");
                SqlExecutionResult[] testResults = TestService.Execute(this.ExecutionContext, this.PrivilegedContext, testActions.TestAction);
            }
            finally
            {
                // Execute the post-test script
                // 
                System.Diagnostics.Trace.WriteLineIf((testActions.PosttestAction != null), "Executing post-test script...");
                SqlExecutionResult[] posttestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PosttestAction);
            }
        }


        private SqlDatabaseTestActions dbo_tsp_UpdateUserTest_NewNameData;
        private SqlDatabaseTestActions dbo_tsp_UpdateUserTest_NewAllData;
    }
}
