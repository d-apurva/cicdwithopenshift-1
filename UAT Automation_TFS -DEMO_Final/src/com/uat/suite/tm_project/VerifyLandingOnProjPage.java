


/*Test Case:-  Verify user (Admin /Project Lead/StakeHolder/Test Manager/Tester) lands on 'Projects' page when click on 
             the 'Test Management' tab.
             
Result :-    1. User should be logged in as an ' (Admin /Project Lead/StakeHolder/Test Manager) user.
             2. User should land on 'Projects' Page successfully.    */





package com.uat.suite.tm_project;

import org.testng.SkipException;
import org.testng.annotations.AfterMethod;
import org.testng.annotations.AfterTest;
import org.testng.annotations.BeforeTest;
import org.testng.annotations.DataProvider;
import org.testng.annotations.Listeners;
import org.testng.annotations.Test;

import recorder.Utility;

import com.uat.util.TestUtil;

@Listeners(com.uat.listener.TestsListenerAdapter.class)
public class VerifyLandingOnProjPage extends TestSuiteBase {
	
	
	String runmodes[]=null;
	boolean fail=false;
	boolean skip=false;
	boolean isTestPass=true;
	int count=-1;
	boolean isLoginSuccess=false;
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
	
	// Test Case Implementation ...
		
	@Test(dataProvider="getTestData")
	public void verifyLandingOnProjPage(String Role) throws Exception
	{
		count++;
		
		if(!runmodes[count].equalsIgnoreCase("Y")){
			skip=true;
			APP_LOGS.debug("Runmode for test set data no. "+(count+1)+" set to no.........So its skipping Test Set Data No."+(count+1));;

			throw new SkipException("Runmode for test set data no. "+(count+1)+" set to no");
					}
		
		APP_LOGS.debug(" Executing Test Case -> VerifyLandingOnProjPage...");
		
		
		openBrowser();
		
		APP_LOGS.debug("Opening Browser... ");
		
		
		isLoginSuccess = login(Role);
		
		if(isLoginSuccess)
		{
			
			APP_LOGS.debug("Clicking On Test Management Tab ");
			
			try 
			{
				
				// clicking on Test Management Tab
				getElement("UAT_testManagement_Id").click();		
			
				//finding class Attribute Of PROJECTS Tab To Compare with Class Attribute Value which we are providing in AsserEquals			
				//String SubHead_PROJECTS_Highlighted  =getObject("Projects_projectTab").getAttribute("class");
				String highlightedTabClass  =getObject("Projects_projectTab").getAttribute("class");
			
				if (compareStrings(OR.getProperty("Projects_projectTab_Class"),highlightedTabClass))
				{
					APP_LOGS.debug("PROJECTS TAB is Highlighted ");					
				
					APP_LOGS.debug("User " + Role+ " is landing on' Projects' Page successfully ");				
					
				} 
				else 
				{
					fail=true;
					
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "SubHead_PROJECTS_HighlightedError");

				}
				
			} 
			
			catch (Throwable e)
			{
				fail=true;
				assertTrue(false);
				
				TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Exception");
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
		}
		else
			TestUtil.reportDataSetResult(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, "PASS");
		
		skip=false;
		fail=false;
		

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
	

	
	
	


