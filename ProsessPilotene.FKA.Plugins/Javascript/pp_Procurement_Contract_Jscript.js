// JavaScript source code to show/hide sections
function showHideSubcontracts() {
   if  (Xrm.Page.context.client.getClient() == "Mobile") return;
   // Code for CRM for phones and tablets only goes here.

    var selectedValue = Xrm.Page.getAttribute("pp_maincontract").getValue();
    if (selectedValue == 0) {
        Xrm.Page.ui.tabs.get("GeneralTab").sections.get("subcontractSection").setVisible(false);
    }
    else {
        Xrm.Page.ui.tabs.get("GeneralTab").sections.get("subcontractSection").setVisible(true);

    }
}


// JavaScript source code to show/hide sections
function showHideFvtOrSvt() {
   if  (Xrm.Page.context.client.getClient() == "Mobile") return;
   // Code for CRM for phones and tablets only goes here.

    var selectedValue = Xrm.Page.getAttribute("pp_category").getValue();
    if (selectedValue == 778380000) {
        Xrm.Page.ui.tabs.get("GeneralTab").sections.get("svtSection").setVisible(false);
        Xrm.Page.ui.tabs.get("GeneralTab").sections.get("fvtSection").setVisible(true);
    }
    else if (selectedValue == 778380001) {
        Xrm.Page.ui.tabs.get("GeneralTab").sections.get("svtSection").setVisible(true);
        Xrm.Page.ui.tabs.get("GeneralTab").sections.get("fvtSection").setVisible(false);
    }

    else {
        Xrm.Page.ui.tabs.get("GeneralTab").sections.get("svtSection").setVisible(false);
        Xrm.Page.ui.tabs.get("GeneralTab").sections.get("fvtSection").setVisible(false);

    }
}

// (OnChange and OnLoad) Show related fields if Data Processing Agreement = Yes
function showHideDataProcessAgreement() {
    if  (Xrm.Page.context.client.getClient() == "Mobile") return;
   
    var selectedValue = Xrm.Page.getAttribute("pp_dataprocessingagreement").getValue();
    //console.log("pp_dataprocessingagreement = " + selectedValue);
    if (selectedValue == 778380000) {
        Xrm.Page.getControl("pp_agreementattached").setVisible(true);
        Xrm.Page.getControl("pp_agreementcomment").setVisible(true);
        Xrm.Page.getAttribute("pp_agreementattached").setRequiredLevel("required");
        Xrm.Page.getAttribute("pp_agreementcomment").setRequiredLevel("required");
    }
    else {
        Xrm.Page.getControl("pp_agreementattached").setVisible(false);
        Xrm.Page.getControl("pp_agreementcomment").setVisible(false);
        Xrm.Page.getAttribute("pp_agreementattached").setRequiredLevel("none");
        Xrm.Page.getAttribute("pp_agreementcomment").setRequiredLevel("none");
    }		
}

function SetDocumentFrame() {
   if  (Xrm.Page.context.client.getClient() == "Mobile") return;
   // Do not work on phones and tablets 

    var fType = Xrm.Page.ui.getFormType();
    if (fType != 1 && fType != 5) {
        // bare dersom form type IKKE create or quick create


        //You can see what the url should be by navigating to the 'Documents' area under related records, viewing the page soure
        //and looking for 'areaSPDocuments'. The formid appears to be nothing more than a random guid value and not tied to anything 
        //specific in your org. 

        //Use: Make sure Document Management is enabled for the entity (helps to turn on automatic folder creation)
        //     Add a web resource with this code to the form 
        //     Execute this function during the form's OnLoad event

        var url = Xrm.Page.context.getClientUrl() +
            "/userdefined/areas.aspx?formid=ab44efca-df12-432e-a74a-83de61c3f3e9&inlineEdit=1&navItemName=Documents&oId=%7b" +
            Xrm.Page.data.entity.getId().replace("{", "").replace("}", "") + "%7d&oType=" +
            Xrm.Page.context.getQueryStringParameters().etc +
            "&pagemode=iframe&rof=true&security=852023&tabSet=areaSPDocuments&theme=Outlook15White";

        Xrm.Page.getControl("IFRAME_sp_documents").setSrc(url); //Replace IFRAME_sp_documents with actual IFRAME name
        Xrm.Page.getControl("pp_shortcontent").setFocus();
    }
}

// Jscript to set default Business Unit equal to business unit of logged on user
function GetAndSetUsersBU() {
    var fType = Xrm.Page.ui.getFormType();
    if (fType == 1 || fType == 5) {
        // bare dersom form type create or quick create

        // **************************************************************************************
        //  21.10.2011 Morten Skau  -   JSCRIPT for getting info about current user
        //  Call is asynchronious and will call the need function upon success or failure
        // **************************************************************************************
        retrieveRecord(Xrm.Page.context.getUserId().substring(1, 37), "SystemUserSet", UserRetrievedSuccess, UserRetrievedError, "SystemUserId,BusinessUnitId,FullName");


        // **************************************************************************************
        //  21.10.2011 Morten Skau  -   JSCRIPT to be ran after USER has been retrieved w Error
        //  Call is asynchronious 
        // **************************************************************************************
        function UserRetrievedError(XmlHttpRequest, textStatus, errorThrown) {
            alert('Error: ' + errorThrown);
        }


        // **************************************************************************************
        //  21.10.2011 Morten Skau  -   JSCRIPT to be ran after USER has been retrieved w Success
        //  Call is asynchronious 
        // **************************************************************************************
        function UserRetrievedSuccess(data, textStatus, XmlHttpRequest) {
            var userId = data.SystemUserId.toLowerCase();               // id of the current user
            var userBuId = data.BusinessUnitId.Id.toLowerCase();        // businessunit of the current user
            var userBuName = data.BusinessUnitId.Name;                  // businessunit of the current user

            if (userBuId != "" && userBuId != null) {
                var value = new Array();
                value[0] = new Object();
                value[0].id = userBuId;
                value[0].name = userBuName;
                value[0].entityType = 'BusinessUnit';
                Xrm.Page.getAttribute("pp_businessunit").setValue(value);
            }
        }
    }
}

//function to close documenttab
function CloseTabOnLoad() {
    var currTabState = Xrm.Page.ui.tabs.get("Dokumenter_section").getDisplayState();
    if (currTabState == "expanded") {
        Xrm.Page.ui.tabs.get("Dokumenter_section").setDisplayState("collapsed");
    }
}

function SetDocumentFrameOnExpand() {
    if  (Xrm.Page.context.client.getClient() == "Mobile") return;
    // Do not work on phones and tablets 
    var currTabState = Xrm.Page.ui.tabs.get("Dokumenter_section").getDisplayState();
    if (currTabState == "expanded") {

        var fType = Xrm.Page.ui.getFormType();
        if (fType != 1 && fType != 5) {
        // bare dersom form type IKKE create or quick create


        //You can see what the url should be by navigating to the 'Documents' area under related records, viewing the page soure
        //and looking for 'areaSPDocuments'. The formid appears to be nothing more than a random guid value and not tied to anything 
        //specific in your org. 

        //Use: Make sure Document Management is enabled for the entity (helps to turn on automatic folder creation)
        //     Add a web resource with this code to the form 
        //     Execute this function during the form's OnLoad event

        var url = Xrm.Page.context.getClientUrl() +
            "/userdefined/areas.aspx?formid=ab44efca-df12-432e-a74a-83de61c3f3e9&inlineEdit=1&navItemName=Documents&oId=%7b" +
            Xrm.Page.data.entity.getId().replace("{", "").replace("}", "") + "%7d&oType=" +
            Xrm.Page.context.getQueryStringParameters().etc +
            "&pagemode=iframe&rof=true&security=852023&tabSet=areaSPDocuments&theme=Outlook15White";

        Xrm.Page.getControl("IFRAME_sp_documents").setSrc(url); //Replace IFRAME_sp_documents with actual IFRAME name
        }
    }
}

//used by scoring of fair trade (etisk handel)
function CalculateScore1() {
    var score = 0;
    if (Xrm.Page.getAttribute("pp_lowcostcountry").getValue()) {
        score++;
    }
    if (Xrm.Page.getAttribute("pp_workintensive").getValue()) {
        score++;
    }
    if (Xrm.Page.getAttribute("pp_violationofrights").getValue()) {
        score++;
    }
    if (Xrm.Page.getAttribute("pp_environmentalrisk").getValue()) {
        score++;
    }
    Xrm.Page.getAttribute("pp_score1").setValue(score);
}
