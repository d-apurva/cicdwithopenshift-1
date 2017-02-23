var groupName = new Array();
var selectedGroup = new Array();
var length = $('#grpul').find('li').length;
for(var i = 0; i<length; i++)
{
	groupName.push($("#grpul").find('li')[i].getAttribute('title'));
}
  $("#divAlert").html('<div id="divHTMLPP"></div>');
  var htmlPP = '<div class="Mediumddl" style="border: 1px solid rgb(204, 204, 204); width: 460px; padding-left: 1px;">'
  +'<ul id="ulItemstesterRolesConf" style="display: inline; list-style-type: none; list-style-position: outside;">'
  +'<li>Below is the list of available Groups. Please select the ones to be deleted.</li></ul></div>';
  for(var i=0;i<groupName.length;i++)
  {
	  var pid = 'pid'+i;
	  htmlPP += '<div style="width: 460px; height: auto;"><li><input class="mslChk" type="checkbox" id="'+pid+'">'+groupName[i]+'</li>'
  }
  +'</div>';
  $("#divHTMLPP").html(htmlPP); 
  $('#divAlert').dialog({ resizable: false,
     modal: true,
     height: 370,
     width:500,
     buttons: {
     "Ok" : function(){
    	 for(var k=0; k<groupName.length;k++)
		 {
			 if($('#pid'+k).is(':checked') == true)
			 {
				 selectedGroup.push(groupName[k]);
			 }
		 }
    	 alert(selectedGroup);
    	 $( this ).dialog( "close" );
     }           
   }
  });