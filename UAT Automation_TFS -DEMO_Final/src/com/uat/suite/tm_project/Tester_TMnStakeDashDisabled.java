/* Test Case:-	1. Login in UAT with Tester user credentials.
				2. Verify the 'Test Management' tab available on Dashboard or any other page.  
				
   Expected Result :-	1. User should be logged in as a 'Tester' user.
						2. On all pages, 'Test Management' tab should be disabled for a Tester user. (Hence Tester should not be able to land on 'Projects' page)


*/

package com.uat.suite.tm_project;


import org.openqa.selenium.WebElement;
import org.testng.SkipException;
import org.testng.annotations.AfterMethod;
import org.testng.annotations.AfterTest;
import org.testng.annotations.BeforeTest;
import org.testng.annotations.DataProvider;
import org.testng.annotations.Test;

import recorder.Utility;

import com.uat.util.TestUtil;


//@Listeners(com.uat.listener.TestsListenerAdapter.class)
public class Tester_TMnStakeDashDisabled extends TestSuiteBase  {
	
	String runmodes[]=null;
	boolean fail=false;
	boolean skip=false;
	boolean isTestPass=true;
	int count=-1;
	boolean isLoginSuccess=false;
	String comments;
	Utility utilRecorder = new Utility();
	
	// Runmode of test case in a suite
		@BeforeTest
		public void checkTestSkip() throws Exception{
			
			APP_LOGS.debug("Beginning test case '"+this.getClass().getSimpleName()+"'.");
			
			if(!TestUtil.isTestCaseRunnable(TM_projectSuiteXls,this.getClass().getSimpleName())){
				APP_LOGS.debug("Skipping Test Case"+this.getClass().getSimpleName()+" as runmode set to NO");//logs
				throw new SkipException("Skipping Test Case"+this.getClass().getSimpleName()+" as runmode set to NO");//reports
			}
			runmodes=TestUtil.getDataSetRunmodes(TM_projectSuiteXls, this.getClass().getSimpleName());
			utilRecorder.startRecording(System.getProperty("user.dir")+"\\Videos\\"+extractPackageName(this.getClass().getPackage().toString())+"\\"+this.getClass().getSimpleName());

		}
	
	
	// Test Case Implementation 	
		
	@Test(dataProvider="getTestData")
	public void testerTmAndStakeDashDisabled(String role) throws Exception, InterruptedException
	{
        count++;
        comments="";
        
		if(!runmodes[count].equalsIgnoreCase("Y")){
			skip=true;
			throw new SkipException("Runmode for test set data no. "+(count+1)+" set to no");
		}
		
		APP_LOGS.debug(" Executing Test Tester_TMnStakeDashDisabled...");
		
		
		openBrowser();
		
		APP_LOGS.debug("Opening Browser... ");
		
		
		isLoginSuccess = login(role);
		
		
		if(isLoginSuccess)
		{
		
				APP_LOGS.debug("Dashoboard Page is displayed...");
		
		
				APP_LOGS.debug("Navigating Cursor On Test Management Tab Link...");
			
			
				//Checking whether Test Management Tab is Disabled Or Not
				
				try
				{
					WebElement TM_Tab_Disabled = getElement("UAT_testManagement_Id");
			
					String[] isTMDisabled=TM_Tab_Disabled.getAttribute("style").split(":");
					
					
					if (!isTMDisabled[1].equals(" gray;"))
					{	
							fail=true;
							assertTrue(false);
							comments="Test Management Tab is not disabled(Fail). ";
							
							TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Testers_TMnStakeDashDisabledError");
							
							
							APP_LOGS.debug("Test Management Tab link: Enabled");
							
							
							APP_LOGS.debug(" Test Case Failed ");
					}
					else
					{	
							comments="Test Management Tab is disabled(Pass). ";
							
							APP_LOGS.debug("Test Management Tab link: Disabled");
							
							APP_LOGS.debug("For User " + role + ": Test Management Tab is showing Disabled Successfully ");
							
					}
				}
			
			
				catch(Throwable t)
				{	
					fail=true;
					assertTrue(false);
					comments="Exception occured while checking test management tab. ";
					t.printStackTrace();
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Testers_TMnStakeDashDisabledError");
				}
			
			
				APP_LOGS.debug("Navigating Cursor On Stakeholder Dashboard  Tab Link...");
			
				
				//Checking whether Stakeholder Dashboard Tab is Disabled Or Not
				try
				{
					WebElement StakeDash_Tab_Disabled = getElement("UAT_stakeholderDashboard_Id");
			
					String[] isStakeDashDisabled=StakeDash_Tab_Disabled.getAttribute("style").split(":");
					
					
					if (!isStakeDashDisabled[1].equals(" grey;"))
					{
							fail=true;
							assertTrue(false);
							comments+="Stakeholder Dashboard Tab is not disabled(Fail). ";
							TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Testers_TMnStakeDashDisabledError");
							
							
							APP_LOGS.debug("Stakeholder Dashboard Tab Link: Enabled");
							
							
							APP_LOGS.debug(" Test Case Failed ");
					}
					else
					{	
						comments+="Stakeholder Dashboard Tab is disabled(Pass). ";
							
							APP_LOGS.debug("Stakeholder Dashboard Tab Link: Disabled");
							
							APP_LOGS.debug("For User " + role + ": Stakeholder Dashboard Tab is showing Disabled Successfully ");
							
					}
				}
			
			
				catch(Throwable t)
				{	
					fail=true;
					assertTrue(false);
					comments+="Exception occured while checking stakeholder dashboard tab. ";
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Testers_TMnStakeDashDisabledError");
					
					t.printStackTrace();
				}
			
			
	
			
				APP_LOGS.debug("Closing Browser... ");
			
			
				closeBrowser();
	
		}
	
		else 
		{
			isLoginSuccess=false;
			APP_LOGS.debug("Login Not Successful");
		}		
	

	
}
	
	
	@AfterMethod
	public void reportDataSetResult(){
		if(skip)
			TestUtil.reportDataSetResult(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, "SKIP");
		
		else if(!isLoginSuccess){
			isTestPass=false;
			TestUtil.reportDataSetResult(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, "Login UnSuccessfull");
		}
		else if(fail){
			isTestPass=false;
			TestUtil.reportDataSetResult(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, "FAIL");
			TestUtil.printComments(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, comments);
		}
		else{
			TestUtil.reportDataSetResult(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, "PASS");
			TestUtil.printComments(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, comments);
		}
			
		
		skip=false;
		fail=false;

		System.gc();

	}
	
	@AfterTest
	public void reportTestResult() throws Exception{
		if(isTestPass)
			TestUtil.reportDataSetResult(TM_projectSuiteXls, "Test Cases", TestUtil.getRowNum(TM_projectSuiteXls,this.getClass().getSimpleName()), "PASS");
		else
			TestUtil.reportDataSetResult(TM_projectSuiteXls, "Test Cases", TestUtil.getRowNum(TM_projectSuiteXls,this.getClass().getSimpleName()), "FAIL");
		
		utilRecorder.stopRecording();
	}
	
	
	
	@DataProvider
	public Object[][] getTestData(){
		return TestUtil.getData(TM_projectSuiteXls, this.getClass().getSimpleName()) ;
	}

}
