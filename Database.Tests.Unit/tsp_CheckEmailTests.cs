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
    public class tsp_CheckEmailTests : SqlDatabaseTestClass
    {

        public tsp_CheckEmailTests()
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
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction dbo_tsp_CheckEmailTest_Available_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(tsp_CheckEmailTests));
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition noMatchingEmails;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction testInitializeAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction dbo_tsp_CheckEmailTest_Unavailable_TestAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition emailUsed;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction testCleanupAction;
            this.dbo_tsp_CheckEmailTest_AvailableData = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            this.dbo_tsp_CheckEmailTest_UnavailableData = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            dbo_tsp_CheckEmailTest_Available_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            noMatchingEmails = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            testInitializeAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            dbo_tsp_CheckEmailTest_Unavailable_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            emailUsed = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            testCleanupAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            // 
            // dbo_tsp_CheckEmailTest_Available_TestAction
            // 
            dbo_tsp_CheckEmailTest_Available_TestAction.Conditions.Add(noMatchingEmails);
            resources.ApplyResources(dbo_tsp_CheckEmailTest_Available_TestAction, "dbo_tsp_CheckEmailTest_Available_TestAction");
            // 
            // noMatchingEmails
            // 
            noMatchingEmails.ColumnNumber = 1;
            noMatchingEmails.Enabled = true;
            noMatchingEmails.ExpectedValue = "0";
            noMatchingEmails.Name = "noMatchingEmails";
            noMatchingEmails.NullExpected = false;
            noMatchingEmails.ResultSet = 1;
            noMatchingEmails.RowNumber = 1;
            // 
            // testInitializeAction
            // 
            resources.ApplyResources(testInitializeAction, "testInitializeAction");
            // 
            // dbo_tsp_CheckEmailTest_Unavailable_TestAction
            // 
            dbo_tsp_CheckEmailTest_Unavailable_TestAction.Conditions.Add(emailUsed);
            resources.ApplyResources(dbo_tsp_CheckEmailTest_Unavailable_TestAction, "dbo_tsp_CheckEmailTest_Unavailable_TestAction");
            // 
            // emailUsed
            // 
            emailUsed.ColumnNumber = 1;
            emailUsed.Enabled = true;
            emailUsed.ExpectedValue = "1";
            emailUsed.Name = "emailUsed";
            emailUsed.NullExpected = false;
            emailUsed.ResultSet = 1;
            emailUsed.RowNumber = 1;
            // 
            // testCleanupAction
            // 
            resources.ApplyResources(testCleanupAction, "testCleanupAction");
            // 
            // dbo_tsp_CheckEmailTest_AvailableData
            // 
            this.dbo_tsp_CheckEmailTest_AvailableData.PosttestAction = null;
            this.dbo_tsp_CheckEmailTest_AvailableData.PretestAction = null;
            this.dbo_tsp_CheckEmailTest_AvailableData.TestAction = dbo_tsp_CheckEmailTest_Available_TestAction;
            // 
            // dbo_tsp_CheckEmailTest_UnavailableData
            // 
            this.dbo_tsp_CheckEmailTest_UnavailableData.PosttestAction = null;
            this.dbo_tsp_CheckEmailTest_UnavailableData.PretestAction = null;
            this.dbo_tsp_CheckEmailTest_UnavailableData.TestAction = dbo_tsp_CheckEmailTest_Unavailable_TestAction;
            // 
            // tsp_CheckEmailTests
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
        public void dbo_tsp_CheckEmailTest_Available()
        {
            SqlDatabaseTestActions testActions = this.dbo_tsp_CheckEmailTest_AvailableData;
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
        public void dbo_tsp_CheckEmailTest_Unavailable()
        {
            SqlDatabaseTestActions testActions = this.dbo_tsp_CheckEmailTest_UnavailableData;
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

        private SqlDatabaseTestActions dbo_tsp_CheckEmailTest_AvailableData;
        private SqlDatabaseTestActions dbo_tsp_CheckEmailTest_UnavailableData;
    }
}
