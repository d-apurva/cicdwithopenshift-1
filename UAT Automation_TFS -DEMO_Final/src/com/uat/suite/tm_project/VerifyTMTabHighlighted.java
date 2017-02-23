/*Test Case:- Projects : Verify the 'Test Management' tabis highlighted when user lands on'Projects' page of 'Test Management'.

Expected Result :- 1. User should be logged in as per the valid user credentials entered.
2. User should land on 'Projects' Page successfully. 
3. 'Test Management' tab should be highlighted.
4. 'Test Management' tab should be highlighted.*/

package com.uat.suite.tm_project;

import java.io.IOException;

import org.openqa.selenium.WebElement;
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
public class VerifyTMTabHighlighted extends TestSuiteBase  {
	
	String runmodes[]=null;
	boolean fail=false;
	boolean skip=false;
	boolean isTestPass=true;
	int count=-1;
	boolean isLoginSuccess=false;
	String TM_Tab_Highlighted;
	int errorCount=0;
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
	public void verifyTMTabHighlighted(String Role) throws Exception
	{
		count++;
		
		if(!runmodes[count].equalsIgnoreCase("Y"))
		{
			skip=true;
			APP_LOGS.debug("Runmode for test set data no. "+(count+1)+" set to no.........So its skipping Test Set Data No."+(count+1));

			throw new SkipException("Runmode for test set data no. "+(count+1)+" set to no");
		}
		
		APP_LOGS.debug(" Executing Test Case -> VerifyTMTabHighlighted... for role "+Role);
		

		openBrowser();
		
		APP_LOGS.debug("Opening Browser... ");
		
		
		isLoginSuccess = login(Role);
		
		if(isLoginSuccess)
		
		{
			try 
			{
				APP_LOGS.debug("Clicking On Test Management Tab ");
				
				navigateAndVerify(getElement("UAT_testManagement_Id"));
				Thread.sleep(2000);
				
				
				APP_LOGS.debug("Clicking On TEST PASSES Tab ...");
								
				navigateAndVerify(getElement("TM_testPassesTab_Id"));
				Thread.sleep(2000);	
					
				
				APP_LOGS.debug("Clicking On TESTERS Tab ...");
				
				navigateAndVerify(getElement("TM_testersTab_Id"));
				Thread.sleep(2000);		
				
				
				APP_LOGS.debug("Clicking On TEST CASES Tab ...");
				
				navigateAndVerify(getElement("TM_testCasesTab_Id"));
				Thread.sleep(2000);
				
				
				APP_LOGS.debug("Clicking On TEST STEPS Tab ...");								
				navigateAndVerify(getElement("TM_testStepsTab_Id"));
				Thread.sleep(2000);	
				
				
				APP_LOGS.debug("Clicking On ATTACHMENTS Tab ...");			
				
				navigateAndVerify(getElement("TM_attachmentsTab_Id"));				
				Thread.sleep(2000);
			} 				
			catch (Throwable e)
			{
				fail= true;
				APP_LOGS.debug("Exception Occured");
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
	
	
	
	private void navigateAndVerify(WebElement elementToClickOn) throws IOException, InterruptedException
	{
		Thread.sleep(1000);
		elementToClickOn.click();
		
		Thread.sleep(1000);
		
		TM_Tab_Highlighted  =getElement("UAT_testManagement_Id").getAttribute("class");
		
		if (compareStrings(OR.getProperty("UAT_testManagementTab_Class"), TM_Tab_Highlighted)) 
		{
			APP_LOGS.debug("Test Management Tab is highlighted...");		
					
		}
		else
		{
			fail = true;
			errorCount++;
			TestUtil.takeScreenShot(this.getClass().getSimpleName(), "VerifyTMTabHighlightedError"+errorCount);
						
			APP_LOGS.debug("Test Management Tab is not highlighted...");
			
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
	

	
	
	


