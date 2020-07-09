﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Dwapi.ExtractsManagement.Infrastructure.Migrations
{
    public partial class JaroReview : Migration
    {
       protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider.ToLower().Contains("SqlServer".ToLower()))
            {
                migrationBuilder.Sql(@"
create view vMasterPatientIndicesJaroV2
                as
                SELECT 

A.PatientPk,A.SiteCode,A.FacilityName,
 A.Id, A.Serial, A.FirstName, A.MiddleName, A.LastName, A.FirstName_Normalized, A.MiddleName_Normalized, A.LastName_Normalized, A.PatientPhoneNumber, 
                         A.PatientAlternatePhoneNumber, A.Gender, A.DOB, A.MaritalStatus, A.PatientSource, A.PatientCounty, A.PatientSubCounty, A.PatientVillage, A.PatientID, A.National_ID, A.NHIF_Number, 
                         A.Birth_Certificate, A.CCC_Number, A.TB_Number, A.ContactName, A.ContactRelation, A.ContactPhoneNumber, A.ContactAddress, A.DateConfirmedHIVPositive, A.StartARTDate, 
                         A.StartARTRegimenCode, A.StartARTRegimenDesc, A.dmFirstName, A.dmLastName, A.sxFirstName, A.sxLastName, A.sxPKValue, A.dmPKValue, A.sxdmPKValue, A.sxMiddleName, 
                         A.dmMiddleName, A.sxPKValueDoB, A.dmPKValueDoB, A.sxdmPKValueDoB, A.DateExtracted, A.Processed, A.QueueId, A.Status, A.StatusDate,B.sxdmPKValueDoB sxdmPKValueDoBOther
                FROM [MasterPatientIndices] A
                INNER JOIN
            (
                SELECT  sxdmPKValueDoB, COUNT(*) Number  from[MasterPatientIndices]
            GROUP BY sxdmPKValueDoB having COUNT(*) > 1
                ) AS  B
            ON A.sxdmPKValueDoB = B.sxdmPKValueDoB
                ");
            }

            if (migrationBuilder.ActiveProvider.ToLower().Contains("MySql".ToLower()))
            {

                migrationBuilder.Sql(@"
create view vMasterPatientIndicesJaroV2
                as
                SELECT 

A.PatientPk,A.SiteCode,A.FacilityName,
 A.Id, A.Serial, A.FirstName, A.MiddleName, A.LastName, A.FirstName_Normalized, A.MiddleName_Normalized, A.LastName_Normalized, A.PatientPhoneNumber, 
                         A.PatientAlternatePhoneNumber, A.Gender, A.DOB, A.MaritalStatus, A.PatientSource, A.PatientCounty, A.PatientSubCounty, A.PatientVillage, A.PatientID, A.National_ID, A.NHIF_Number, 
                         A.Birth_Certificate, A.CCC_Number, A.TB_Number, A.ContactName, A.ContactRelation, A.ContactPhoneNumber, A.ContactAddress, A.DateConfirmedHIVPositive, A.StartARTDate, 
                         A.StartARTRegimenCode, A.StartARTRegimenDesc, A.dmFirstName, A.dmLastName, A.sxFirstName, A.sxLastName, A.sxPKValue, A.dmPKValue, A.sxdmPKValue, A.sxMiddleName, 
                         A.dmMiddleName, A.sxPKValueDoB, A.dmPKValueDoB, A.sxdmPKValueDoB, A.DateExtracted, A.Processed, A.QueueId, A.Status, A.StatusDate,B.sxdmPKValueDoB sxdmPKValueDoBOther

                FROM MasterPatientIndices A
                INNER JOIN vMasterPatientIndicesJaroB AS  B
             ON A.sxdmPKValueDoB = B.sxdmPKValueDoB
                ");
            }

            if (migrationBuilder.ActiveProvider.ToLower().Contains("sqlite".ToLower()))
            {
                migrationBuilder.Sql(@"
create view vMasterPatientIndicesJaroV2
                as
                SELECT 

A.PatientPk,A.SiteCode,A.FacilityName,
 A.Id, A.Serial, A.FirstName, A.MiddleName, A.LastName, A.FirstName_Normalized, A.MiddleName_Normalized, A.LastName_Normalized, A.PatientPhoneNumber, 
                         A.PatientAlternatePhoneNumber, A.Gender, A.DOB, A.MaritalStatus, A.PatientSource, A.PatientCounty, A.PatientSubCounty, A.PatientVillage, A.PatientID, A.National_ID, A.NHIF_Number, 
                         A.Birth_Certificate, A.CCC_Number, A.TB_Number, A.ContactName, A.ContactRelation, A.ContactPhoneNumber, A.ContactAddress, A.DateConfirmedHIVPositive, A.StartARTDate, 
                         A.StartARTRegimenCode, A.StartARTRegimenDesc, A.dmFirstName, A.dmLastName, A.sxFirstName, A.sxLastName, A.sxPKValue, A.dmPKValue, A.sxdmPKValue, A.sxMiddleName, 
                         A.dmMiddleName, A.sxPKValueDoB, A.dmPKValueDoB, A.sxdmPKValueDoB, A.DateExtracted, A.Processed, A.QueueId, A.Status, A.StatusDate,B.sxdmPKValueDoB sxdmPKValueDoBOther

                FROM MasterPatientIndices A
                INNER JOIN vMasterPatientIndicesJaroB AS  B
             ON A.sxdmPKValueDoB = B.sxdmPKValueDoB
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP View vMasterPatientIndicesJaroV2");
        }
    }
}
