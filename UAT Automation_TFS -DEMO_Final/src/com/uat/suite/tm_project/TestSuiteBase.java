package com.uat.suite.tm_project;

import java.io.IOException;

import org.openqa.selenium.By;
import org.testng.SkipException;
import org.testng.annotations.BeforeSuite;

import com.uat.base.TestBaseOld;
import com.uat.util.TestUtil;

public class TestSuiteBase extends TestBaseOld{

	// check if the suite ex has to be skiped
		@BeforeSuite
		public void checkSuiteSkip() throws Exception{
			initialize();
			APP_LOGS.debug("Checking Runmode of TM_Project Suite");
			if(!TestUtil.isSuiteRunnable(suiteXls, "TM_Project Suite")){
				APP_LOGS.debug("Skipped Test Management Suite  as the runmode was set to NO");
				throw new SkipException("RUnmode of Test Management Suite set to NO ... So Skipping all tests in Test Management Suite");
			}
			
		}	
		public void freememory(){
		    Runtime basurero = Runtime.getRuntime(); 
		    basurero.gc();
		}
		
		
		
		public boolean searchProjectInViewAllPage(String project, String version) throws IOException, InterruptedException
		{
			int totalPages;
			int projectsOnPage;
			String gridProject;
			String gridVersion;
			//int projectFoundFlag=0;
			APP_LOGS.debug("Searching Project to Edit");
			
			try
			{
			
				if (getElement("ProjectViewAll_Pagination_Id").findElements(By.xpath("div/span")).size()==3) 
				{
					APP_LOGS.debug("Only 1 page available on View All page.");
					
					totalPages=1;
					
				}
				else 
				{
					APP_LOGS.debug("More than 1 page avaialble on View All page. Calculating total pages.");
					
					totalPages = getElement("ProjectViewAll_Pagination_Id").findElements(By.xpath("div/a")).size();
				}		
				
				for (int i = 0; i < totalPages; i++) 
				{
					projectsOnPage = getElement("ProjectViewAll_Table_Id").findElements(By.xpath("tr")).size();
					
					for (int j = 1; j <= projectsOnPage; j++) 
					{
						gridProject=getObject("ProjectViewAll_projectNameColumn1", "ProjectViewAll_projectNameColumn2", j).getAttribute("title");
						gridVersion= getObject("ProjectViewAll_projectVersionColumn1", "ProjectViewAll_projectVersionColumn2", j).getText();
						
						if (gridProject.equals(project) && gridVersion.equals(version)) 
						{
							/*getObject("ProjectViewAll_editProjectIcon1", "ProjectViewAll_editProjectIcon2", j).click();
							Thread.sleep(1000);*/
							APP_LOGS.debug("Project Found in View All page.");
							return true;
						}
						
						
					}			
					
					if (totalPages>1 && i!=(totalPages-1)) 
					{
						getObject("ProjectViewAll_NextLink").click();
					}
					
				}
				
				//assertTrue(false);
				APP_LOGS.debug("Project Not found in View All page");
				
				return false;
			}
			catch(Throwable t)
			{
				t.printStackTrace();
				//assertTrue(false);
				APP_LOGS.debug("Exception in searchProjectAndEdit(). ");
				return false;
			}
			
		}
		

}

