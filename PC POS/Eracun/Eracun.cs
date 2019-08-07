using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using PCPOS.Eracun.Entities;

namespace PCPOS.Eracun
{
    public class Eracun
    {
        private static Eracun _instance;

        private string demoEndPointURL = @"https://demo.moj-eracun.hr/apis/v2/send";

        private string _userName = "5988";
        private string _password = "12345678910";
        private string _companyId = "47165970760";
        private string _companyBu; // Not a mandatory field
        private string _softwareId = "Test-001";
        private string _file;

        public static Eracun Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Eracun();
                return _instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="faktura"></param>
        public void SendEracun(Faktura faktura)
        {
            _file = GenerateEracunXml(faktura);

                var jsonObject = new
                {
                    Username = _userName,
                    Password = _password,
                    CompanyId = _companyId,
                    CompanyBu = _companyBu,
                    SoftwareId = _softwareId,
                    File = _file
                };

            // Serialize object to string
            var serializer = new JavaScriptSerializer();
            var jsonString = serializer.Serialize(jsonObject);

            // Request
            var request = (HttpWebRequest)WebRequest.Create(demoEndPointURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = jsonString.Length;

            // Response
            using (var stream = request.GetRequestStream())
            {
                stream.Write(Encoding.ASCII.GetBytes(jsonString), 0, jsonString.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="faktura"></param>
        public string GenerateEracunXml(Faktura faktura)
        {
            DataTable DTpodaciTvrtke = classSQL.select_settings("SELECT * FROM podaci_tvrtka", "podaci_tvrtka").Tables[0];
            DataTable DTpartner = Global.Database.GetPartners(faktura.IdPartner.ToString());
            DataTable DTgradTvrtka = Global.Database.GetGrad(DTpodaciTvrtke?.Rows[0]["id_grad"].ToString());
            DataTable DTgradPartner = Global.Database.GetGrad(DTpartner?.Rows[0]["id_grad"].ToString());

            string pozivNaBroj = $"{faktura.Model}{faktura.BrojFakture}{faktura.GodinaFakture}-{faktura.IdFakturirati}";

            // Linija 174 - {DTpodaciTvrtke?.Rows[0]["email"].ToString()}
            // Linija 204 - {DTpartner?.Rows[0]["email"].ToString()}

            string xmlString = $@"<?xml version='1.0' encoding='utf-8'?>
            <OutgoingInvoicesData xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='http://fina.hr/eracun/erp/OutgoingInvoicesData/v3.2'>
                <Header>
                    <SupplierID>{DTpodaciTvrtke?.Rows[0]["oib"].ToString()}</SupplierID>
                    <InvoiceType>1</InvoiceType>
                </Header>
                <OutgoingInvoice>
                    <SupplierInvoiceID>{pozivNaBroj}</SupplierInvoiceID>
                    <BuyerID>{DTpartner?.Rows[0]["oib"].ToString()}</BuyerID>
                    <InvoiceEnvelope>
                        <Invoice xmlns:cbc='urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' xmlns:sac='urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2' xmlns:ext='urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2' xmlns:cac='urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2' xmlns:sig='urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2' xsi:schemaLocation='urn:oasis:names:specification:ubl:schema:xsd:Invoice-2 HRInvoice.xsd' xmlns='urn:oasis:names:specification:ubl:schema:xsd:Invoice-2'>
                            <ext:UBLExtensions>
                                <ext:UBLExtension>
                                    <cbc:ID>HRINVOICE1</cbc:ID>
                                    <cbc:Name>InvoiceIssuePlaceData</cbc:Name>
                                    <ext:ExtensionAgencyID>MINGORP</ext:ExtensionAgencyID>
                                    <ext:ExtensionAgencyName>MINGORP</ext:ExtensionAgencyName>
                                    <ext:ExtensionAgencyURI>MINGORP</ext:ExtensionAgencyURI>
                                    <ext:ExtensionURI>urn:invoice:hr:issueplace</ext:ExtensionURI>
                                    <ext:ExtensionReasonCode>MandatoryField</ext:ExtensionReasonCode>
                                    <ext:ExtensionReason>Mjesto izdavanja računa prema Pravilniku o PDV-u</ext:ExtensionReason>
                                    <ext:ExtensionContent>
                                    <ext:InvoiceIssuePlace>Zagreb</ext:InvoiceIssuePlace>
                                    </ext:ExtensionContent>
                                </ext:UBLExtension>
                                <ext:UBLExtension>
                                    <cbc:ID>HRINVOICE1</cbc:ID>
                                    <cbc:Name>InvoiceIssuerData</cbc:Name>
                                    <ext:ExtensionAgencyID>FINA</ext:ExtensionAgencyID>
                                    <ext:ExtensionAgencyName>FINA</ext:ExtensionAgencyName>
                                    <ext:ExtensionAgencyURI>FINA</ext:ExtensionAgencyURI>
                                    <ext:ExtensionURI>urn:invoice:hr:issuer</ext:ExtensionURI>
                                    <ext:ExtensionReasonCode>MandatoryField</ext:ExtensionReasonCode>
                                    <ext:ExtensionReason>Podaci o izdavatelju prema Zakonu o trgovačkim društvima</ext:ExtensionReason>
                                    <ext:ExtensionContent>
                                    <ext:InvoiceIssuer>Adresa registracije: Gjure Szaba odvojak 10 | OIB: 67573415246 | MB: 01316214 | Tvrtka upisana u Trgovački sud Zagreb | IBAN HR6123900011100324481 | Osnivač i osoba ovlaštena za zastupanje tvrtke: Marko Emer | Temeljni kapital: 20.000kn</ext:InvoiceIssuer>
                                    </ext:ExtensionContent>
                                </ext:UBLExtension>
                                <ext:UBLExtension>
                                    <cbc:ID>HRINVOICE1</cbc:ID>
                                    <cbc:Name>IssuerLogoData</cbc:Name>
                                    <ext:ExtensionAgencyID>FINA</ext:ExtensionAgencyID>
                                    <ext:ExtensionAgencyName>FINA</ext:ExtensionAgencyName>
                                    <ext:ExtensionAgencyURI>FINA</ext:ExtensionAgencyURI>
                                    <ext:ExtensionURI>urn:invoice:hr:issuerlogo</ext:ExtensionURI>
                                    <ext:ExtensionReasonCode>MandatoryField</ext:ExtensionReasonCode>
                                    <ext:ExtensionReason>BASE64 logotipa tvrtke</ext:ExtensionReason>
                                    <ext:ExtensionContent>
                                    <ext:IssuerLogo />
                                    </ext:ExtensionContent>
                                </ext:UBLExtension>
                            </ext:UBLExtensions>
                            <cbc:UBLVersionID>2.1</cbc:UBLVersionID>
                            <cbc:CustomizationID>urn:invoice.hr:ubl-2.1-customizations:FinaInvoice</cbc:CustomizationID>
                            <cbc:ProfileID>MojEracunInvoice</cbc:ProfileID>
                            <cbc:ID>{pozivNaBroj}</cbc:ID>
                            <cbc:CopyIndicator>false</cbc:CopyIndicator>
                            <cbc:IssueDate>{faktura.Datum.ToString("yyyy-MM-dd")}</cbc:IssueDate>
                            <cbc:IssueTime>{faktura.Datum.ToString("HH:mm:ss")}</cbc:IssueTime>
                            <cbc:InvoiceTypeCode listID='UN/ECE 1001' listAgencyID='6' listURI='http://www.unece.org/trade/untdid/d00a/tred/tred1001.htm'>380</cbc:InvoiceTypeCode>
                            <cbc:DocumentCurrencyCode listID='ISO 4217 Alpha' listAgencyID='5' listURI='http://docs.oasis-open.org/ubl/os-UBL-2.1/cl/gc/default/CurrencyCode-2.1.gc'>HRK</cbc:DocumentCurrencyCode>
                            <cac:OrderReference>
                                <cbc:ID>{faktura.Datum.Date.ToString("yyyy-MM-dd")}</cbc:ID>
                                <cbc:IssueDate>{faktura.Datum.Date.ToString("yyyy-MM-dd")}</cbc:IssueDate>
                            </cac:OrderReference>
                            <cac:AccountingSupplierParty>
                              <cac:Party>
                                <cbc:EndpointID>3851886047758</cbc:EndpointID>
                                <cac:PartyName>
                                  <cbc:Name>{DTpodaciTvrtke?.Rows[0]["ime_tvrtke"].ToString()}</cbc:Name>
                                </cac:PartyName>
                                <cac:PostalAddress>
                                  <cbc:StreetName>{DTpodaciTvrtke?.Rows[0]["adresa"].ToString()}</cbc:StreetName>
                                  <cbc:BuildingNumber>{DTpodaciTvrtke?.Rows[0]["adresa"].ToString()}</cbc:BuildingNumber>
                                  <cbc:CityName>{DTpodaciTvrtke?.Rows[0]["poslovnica_grad"].ToString()}</cbc:CityName>
                                  <cbc:PostalZone>{DTgradTvrtka.Rows[0]["posta"].ToString()}</cbc:PostalZone>
                                  <cac:AddressLine>
                                    <cbc:Line>{DTpodaciTvrtke?.Rows[0]["adresa"].ToString()}, {DTgradTvrtka.Rows[0]["posta"].ToString()} {DTgradTvrtka.Rows[0]["grad"].ToString()}</cbc:Line>
                                  </cac:AddressLine>
                                  <cac:Country>
                                    <cbc:IdentificationCode listID='ISO3166-1' listAgencyID='6' listName='Country' listVersionID='0.3' listURI='http://docs.oasis-open.org/ubl/os-ubl-2.0/cl/gc/default/CountryIdentificationCode-2.0.gc' listSchemeURI='urn:oasis:names:specification:ubl:codelist:gc:CountryIdentificationCode-2.0'>HR</cbc:IdentificationCode>
                                    <cbc:Name />
                                  </cac:Country>
                                </cac:PostalAddress>
                                <cac:PartyLegalEntity>
                                  <cbc:RegistrationName>{DTpodaciTvrtke?.Rows[0]["ime_tvrtke"].ToString()}</cbc:RegistrationName>
                                  <cbc:CompanyID>{DTpodaciTvrtke?.Rows[0]["oib"].ToString()}</cbc:CompanyID>
                                </cac:PartyLegalEntity>
                              </cac:Party>
                              <cac:AccountingContact>
                                <cbc:Name>{DTpodaciTvrtke?.Rows[0]["vl"].ToString()}</cbc:Name>
                                <cbc:ElectronicMail>valentino@code-it.hr</cbc:ElectronicMail>
                                <cbc:Note>{DTpodaciTvrtke?.Rows[0]["vl"].ToString()}</cbc:Note>
                              </cac:AccountingContact>
                            </cac:AccountingSupplierParty>
                            <cac:AccountingCustomerParty>
                                <cac:Party>
                                <cbc:EndpointID>3857071886047</cbc:EndpointID>
                                <cac:PartyName>
                                    <cbc:Name>{DTpartner?.Rows[0]["ime_tvrtke"].ToString()}</cbc:Name>
                                </cac:PartyName>
                                <cac:PostalAddress>
                                    <cbc:StreetName>{DTpartner?.Rows[0]["adresa"].ToString()}</cbc:StreetName>
                                    <cbc:BuildingNumber>{DTpartner?.Rows[0]["adresa"].ToString()}</cbc:BuildingNumber>
                                    <cbc:CityName>{DTgradPartner.Rows[0]["grad"].ToString()}</cbc:CityName>
                                    <cbc:PostalZone>{DTgradPartner.Rows[0]["posta"].ToString()}</cbc:PostalZone>
                                    <cac:AddressLine>
                                    <cbc:Line>{DTpartner?.Rows[0]["adresa"].ToString()}, {DTgradPartner.Rows[0]["posta"].ToString()} {DTgradPartner.Rows[0]["grad"].ToString()}</cbc:Line>
                                    </cac:AddressLine>
                                    <cac:Country>
                                    <cbc:IdentificationCode listID='ISO3166-1' listAgencyID='6' listName='Country' listVersionID='0.3' listURI='http://docs.oasis-open.org/ubl/os-ubl-2.0/cl/gc/default/CountryIdentificationCode-2.0.gc' listSchemeURI='urn:oasis:names:specification:ubl:codelist:gc:CountryIdentificationCode-2.0'>HR</cbc:IdentificationCode>
                                    <cbc:Name />
                                    </cac:Country>
                                </cac:PostalAddress>
                                <cac:PartyLegalEntity>
                                    <cbc:RegistrationName>{DTpartner?.Rows[0]["ime_tvrtke"].ToString()}</cbc:RegistrationName>
                                    <cbc:CompanyID>{DTpartner?.Rows[0]["oib"].ToString()}</cbc:CompanyID>
                                </cac:PartyLegalEntity>
                                </cac:Party>
                                <cac:AccountingContact>
                                    <cbc:ElectronicMail>valentino@code-it.hr</cbc:ElectronicMail>
                                </cac:AccountingContact>
                            </cac:AccountingCustomerParty>
                            <cac:PaymentMeans>
                                <cbc:PaymentMeansCode listID='UN/ECE 4461' listAgencyID='6' listName='PaymentMeansCode' listVersionID='D10B' listURI='http://docs.oasis-open.org/ubl/os-UBL-2.1/cl/gc/default/PaymentMeansCode-2.1.gc' listSchemeURI='urn:un:unece:uncefact:codelist:standard:UNECE:PaymentMeansCode:D10B'>42</cbc:PaymentMeansCode>
                                <cbc:PaymentDueDate>{faktura.Datum.Date.ToString("yyyy-MM-dd")}</cbc:PaymentDueDate>
                                <cbc:PaymentChannelCode listAgencyName='CEN/BII'>IBAN</cbc:PaymentChannelCode>
                                <cbc:InstructionID>{pozivNaBroj}</cbc:InstructionID>
                                <cbc:InstructionNote>{faktura.Napomena}</cbc:InstructionNote>
                                <cbc:PaymentID>{(string.IsNullOrWhiteSpace(faktura.Model) ? "HR00" : faktura.Model)}</cbc:PaymentID>
                                <cac:PayeeFinancialAccount>
                                <cbc:ID>{faktura.ZiroRacun}</cbc:ID>
                                <cbc:CurrencyCode listID='ISO 4217 Alpha' listAgencyID='5' listURI='http://docs.oasis-open.org/ubl/os-UBL-2.1/cl/gc/default/CurrencyCode-2.1.gc'>HRK</cbc:CurrencyCode>
                                </cac:PayeeFinancialAccount>
                            </cac:PaymentMeans>
                            {GetInvoiceLines(faktura.Stavke, Convert.ToBoolean(DTpartner.Rows[0]["usustavpdv"].ToString()))}
                        </Invoice>
                    </InvoiceEnvelope>
                    <AttachedDocumentEnvelope>
                        <AttachedDocument xmlns:cbc='urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' xmlns:cac='urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2' xsi:schemaLocation='urn:oasis:names:specification:ubl:schema:xsd:AttachedDocument-2 UBL-AttachedDocument-2.1.xsd' xmlns='urn:oasis:names:specification:ubl:schema:xsd:AttachedDocument-2'>
                        <cbc:ID>{pozivNaBroj}</cbc:ID>
                        <cbc:IssueDate>{faktura.Datum.ToString("yyyy-MM-dd")}</cbc:IssueDate>
                        <cbc:IssueTime>{faktura.Datum.ToString("HH:mm:ss")}</cbc:IssueTime>
                        <cbc:ParentDocumentID>{pozivNaBroj}</cbc:ParentDocumentID>
                        <cac:SenderParty>
                            <cac:PartyName>
                            <cbc:Name>{DTpodaciTvrtke.Rows[0]["ime_tvrtke"].ToString()}</cbc:Name>
                            </cac:PartyName>
                            <cac:PostalAddress>
                            <cac:AddressLine>
                                <cbc:Line>{DTpodaciTvrtke?.Rows[0]["adresa"].ToString()}, {DTgradTvrtka.Rows[0]["posta"].ToString()} {DTgradTvrtka.Rows[0]["grad"].ToString()}</cbc:Line>
                            </cac:AddressLine>
                            <cac:Country>
                                <cbc:IdentificationCode listID='ISO3166-1' listAgencyID='6' listName='Country' listVersionID='0.3' listURI='http://docs.oasis-open.org/ubl/os-ubl-2.0/cl/gc/default/CountryIdentificationCode-2.0.gc' listSchemeURI='urn:oasis:names:specification:ubl:codelist:gc:CountryIdentificationCode-2.0'>HR</cbc:IdentificationCode>
                                <cbc:Name />
                            </cac:Country>
                            </cac:PostalAddress>
                            <cac:PartyLegalEntity>
                            <cbc:RegistrationName>{DTpodaciTvrtke.Rows[0]["ime_tvrtke"].ToString()}</cbc:RegistrationName>
                            <cbc:CompanyID>{DTpodaciTvrtke.Rows[0]["oib"].ToString()}</cbc:CompanyID>
                            </cac:PartyLegalEntity>
                        </cac:SenderParty>
                        <cac:ReceiverParty>
                            <cac:PartyName>
                            <cbc:Name>{DTpartner.Rows[0]["ime_tvrtke"]}</cbc:Name>
                            </cac:PartyName>
                            <cac:PostalAddress>
                            <cac:AddressLine>
                                <cbc:Line>{DTpartner?.Rows[0]["adresa"].ToString()}, {DTgradPartner.Rows[0]["posta"].ToString()} {DTgradPartner.Rows[0]["grad"].ToString()}</cbc:Line>
                            </cac:AddressLine>
                            <cac:Country>
                                <cbc:IdentificationCode listID='ISO3166-1' listAgencyID='6' listName='Country' listVersionID='0.3' listURI='http://docs.oasis-open.org/ubl/os-ubl-2.0/cl/gc/default/CountryIdentificationCode-2.0.gc' listSchemeURI='urn:oasis:names:specification:ubl:codelist:gc:CountryIdentificationCode-2.0'>HR</cbc:IdentificationCode>
                                <cbc:Name />
                            </cac:Country>
                            </cac:PostalAddress>
                            <cac:PartyLegalEntity>
                            <cbc:RegistrationName>{DTpartner.Rows[0]["ime_tvrtke"]}</cbc:RegistrationName>
                            <cbc:CompanyID>{DTpartner.Rows[0]["oib"]}</cbc:CompanyID>
                            </cac:PartyLegalEntity>
                        </cac:ReceiverParty>
                        <cac:Attachment>
                            <cbc:EmbeddedDocumentBinaryObject mimeCode='application/pdf' encodingCode='base64' filename='{pozivNaBroj}.pdf'>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sG+vZpXvAgwAc5+npQplbmRzdHJlYW0KZW5kb2JqCjcgMCBvYmoKPDwvU3VidHlwZS9UcnVlVHlwZS9Gb250RGVzY3JpcHRvciAxMCAwIFIvTGFzdENoYXIgMzIvV2lkdGhzWzI1MF0vQmFzZUZvbnQvVGltZXNOZXdSb21hblBTTVQvRmlyc3RDaGFyIDMyL0VuY29kaW5nL1dpbkFuc2lFbmNvZGluZy9UeXBlL0ZvbnQ+PgplbmRvYmoKOCAwIG9iago8PC9TdWJ0eXBlL1RydWVUeXBlL0ZvbnREZXNjcmlwdG9yIDExIDAgUi9MYXN0Q2hhciAxMjEvV2lkdGhzWzI1MCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgNjg4IDAgNTE2IDAgMCAwIDAgMCAwIDAgMCAwIDU3MiAwIDAgMCA1MTMgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgNTQyIDAgMCA1ODcgMCAwIDAgNTgxIDI0NCAwIDAgMCA4OTIgMCAwIDAgMCAwIDQyMyAwIDU4MSAwIDAgMCA1MDRdL0Jhc2VGb250L0hBQkpJRytNSU5JVHlwZVJlZ3VsYXItUmVndWxhci9GaXJzdENoYXIgMzIvRW5jb2RpbmcvV2luQW5zaUVuY29kaW5nL1R5cGUvRm9udD4+CmVuZG9iago5IDAgb2JqCjw8L09QTSAxL09QIGZhbHNlL29wIGZhbHNlL1R5cGUvRXh0R1N0YXRlL1NBIGZhbHNlL1NNIDAuMDI+PgplbmRvYmoKMTAgMCBvYmoKPDwvU3RlbVYgODIvRm9udE5hbWUvVGltZXNOZXdSb21hblBTTVQvRm9udFN0cmV0Y2gvTm9ybWFsL0ZvbnRXZWlnaHQgNDAwL0ZsYWdzIDM0L0Rlc2NlbnQgLTIxNi9Gb250QkJveFstNTY4IC0zMDcgMjAwMCAxMDA3XS9Bc2NlbnQgODkxL0ZvbnRGYW1pbHkoVGltZXMgTmV3IFJvbWFuKS9DYXBIZWlnaHQgNjU2L1hIZWlnaHQgLTU0Ni9UeXBlL0ZvbnREZXNjcmlwdG9yL0l0YWxpY0FuZ2xlIDA+PgplbmRvYmoKMTEgMCBvYmoKPDwvU3RlbVYg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7kXaVBNs7+vzW+NS3jjx2KperQ6uG67ZubpwPBdgAIaUvb4KCmVuZHN0cmVhbQplbmRvYmoKMSAwIG9iago8PC9UeXBlL09DRy9OYW1lKERpc2NsYWltZXIgXChOTiAxNTAvMDVcKSkvVXNhZ2U8PC9QcmludDw8L1N1YnR5cGUvUHJpbnQvUHJpbnRTdGF0ZS9PTj4+L1ZpZXc8PC9WaWV3U3RhdGUvT0ZGPj4+Pj4+CmVuZG9iago1IDAgb2JqCjw8L1R5cGUvUGFnZXMvQ291bnQgMS9LaWRzWzYgMCBSXT4+CmVuZG9iagoxMyAwIG9iago8PC9UeXBlL0NhdGFsb2cvUGFnZXMgNSAwIFIvT0NQcm9wZXJ0aWVzPDwvT0NHc1sxIDAgUl0vRDw8L09yZGVyWzEgMCBSXS9OYW1lKERpc2NsYWltZXIgXChOTiAxNTAvMDVcKSkvQVNbPDwvRXZlbnQvVmlldy9DYXRlZ29yeVsvVmlld10vT0NHc1sxIDAgUl0+Pjw8L0V2ZW50L1ByaW50L0NhdGVnb3J5Wy9QcmludF0vT0NHc1sxIDAgUl0+Pl0vTGlzdE1vZGUvVmlzaWJsZVBhZ2VzPj4+Pj4+CmVuZG9iagoxNCAwIG9iago8PC9Qcm9kdWNlcihpVGV4dFNoYXJwkiA1LjUuMTEgqTIwMDAtMjAxNyBpVGV4dCBHcm91cCBOViBcKEFHUEwtdmVyc2lvblwpKS9DcmVhdGlvbkRhdGUoRDoyMDE3MDkyMTE0MDExMyswMicwMCcpL01vZERhdGUoRDoyMDE3MDkyMTE0MDExMyswMicwMCcpPj4KZW5kb2JqCnhyZWYKMCAxNQowMDAwMDAwMDAwIDY1NTM1IGYgCjAwMDAwMTA4NTQgMDAwMDAgbiAKMDAwMDAwMTM1NCAwMDAwMCBuIAowMDAwMDAwNTQ3IDAwMDAwIG4gCjAwMDAwMDAwMTUgMDAwMDAgbiAKMDAwMDAxMDk4NSAwMDAwMCBuIAowMDAwMDAwMzg1IDAwMDAwIG4gCjAwMDAwMDE4MzEgMDAwMDAgbiAKMDAwMDAwMTk4OSAwMDAwMCBuIAowMDAwMDAyMzYzIDAwMDAwIG4gCjAwMDAwMDI0MzkgMDAwMDAgbiAKMDAwMDAwMjY4MiAwMDAwMCBuIAowMDAwMDAyOTU4IDAwMDAwIG4gCjAwMDAwMTEwMzYgMDAwMDAgbiAKMDAwMDAxMTI3NiAwMDAwMCBuIAp0cmFpbGVyCjw8L1NpemUgMTUvUm9vdCAxMyAwIFIvSW5mbyAxNCAwIFIvSUQgWzwwNWNjMTA3Y2ZkMjg0NTBjZjExNWE2ZjU1NWQzMjZiZT48MDVjYzEwN2NmZDI4NDUwY2YxMTVhNmY1NTVkMzI2YmU+XT4+CiVpVGV4dC01LjUuMTEKc3RhcnR4cmVmCjExNDQwCiUlRU9GCgoxNyAwIG9iago8PC9GVC9TaWcvVChTaWduYXR1cmUxKS9WIDE1IDAgUi9GIDEzMi9UeXBlL0Fubm90L1N1YnR5cGUvV2lkZ2V0L1JlY3RbMCAwIDAgMF0vQVA8PC9OIDE2IDAgUj4+L1AgNiAwIFIvRFI8PD4+Pj4KZW5kb2JqCjE1IDAgb2JqCjw8L1R5cGUvU2lnL0ZpbHRlci9BZG9iZS5QUEtMaXRlL1N1YkZpbHRlci9hZGJlLnBrY3M3LmRldGFjaGVkL1JlYXNvbij+/wBWAGoAZQByAG8AZABvAHMAdABvAGoAbgBvAHMAdAAgAHMAYQBkAHIBfgBhAGoAYQAgAGkAIABkAG8AawBhAHoAaQB2AGEAbgBqAGUAIABwAG8AcwB0AG8AagBhAG4AbwBzAHQAaQAgAHUAIAB2AHIAZQBtAGUAbgB1ACAAcABvAHQAcABpAHMAaQB2AGEAbgBqAGEAIABcKABaAGEAawBvAG4AIABvACAAZQBsAGUAawB0AHIAbwBuAGkBXHIAawBvAGoAIABpAHMAcAByAGEAdgBpACAATgBOACAAMQA1ADAALwAwADUAXCkAIAB8ACAAVABoAGUAIABjAHIAZQBkAGkAYgBpAGwAaQB0AHkAIABvAGYAIABjAG8AbgB0AGUAbgB0ACAAYQBuAGQAIABwAHIAbwBvAGYAIABvAGYAIABlAHgAaQBzAHQAZQBuAGMAZQAgAGEAdAAgAGEAIAB0AGkAbQBlACAAbwBmACAAcwBpAGcAbgBpAG4AZwAgAFwoAEwAYQB3ACAAbwBuACAARQBsAGUAYwB0AHIAbwBuAGkAYwAgAEQAbwBjAHUAbQBlAG4AdAAgAE4ATgAgADEANQAwAC8AMAA1AFwpKS9Mb2NhdGlvbihaYWdyZWIsIEhydmF0c2thIFwoQ3JvYXRpYVwpKS9Qcm9wX0J1aWxkPDwvQXBwPDwvTmFtZShpVGV4dFNoYXJwkiA1LjUuMTEgqTIwMDAtMjAxNyBpVGV4dCBHcm91cCBOViBcKEFHUEwtdmVyc2lvblwpKT4+Pj4vQ29udGFjdEluZm8oKS9NKEQ6MjAxNzA5MjExNDAxMTMrMDInMDAnKS9CeXRlUmFuZ2UgWzAgMTI4NDkgMzc2MTkgMTQ4MCBdICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAvQ29udGVudHMgPDMwODIxMmI0MDYwOTJhODY0ODg2ZjcwZDAxMDcwMmEwODIxMmE1MzA4MjEyYTEwMjAxMDEzMTBmMzAwZDA2MDk2MDg2NDgwMTY1MDMwNDAyMDEwNTAwMzAwYjA2MDkyYTg2NDg4NmY3MGQwMTA3MDFhMDgyMDRjYTMwODIwNGM2MzA4MjAzYWVhMDAzMDIwMTAyMDIwNDNmMmE1NGYyMzAwZDA2MDkyYTg2NDg4NmY3MGQwMTAxMDUwNTAwMzAyYTMxMGIzMDA5MDYwMzU1MDQwNjEzMDI0ODUyMzEwZDMwMGIwNjAzNTUwNDBhMTMwNDQ2NDk0ZTQxMzEwYzMwMGEwNjAzNTUwNDBiMTMwMzUyNDQ0MzMwMWUxNzBkMzEzNDMwMzIzMTMyMzEzNzMyMzQzMzMwNWExNzBkMzEzOTMwMzIzMTMyMzEzNzM1MzQzMzMwNWEzMDY5MzEwYjMwMDkwNjAzNTUwNDA2MTMwMjQ4NTIzMTMzMzAzMTA2MDM1NTA0MGEwYzJhNDU0YzQ1NGI1NDUyNGY0ZTQ5YzQ4YzRiNDkyMDUyNDFjNDhjNTU0ZTQ5MjA0NDJlNGYyZTRmMmUyMDQ4NTIzNDMyMzgzODM5MzIzNTMwMzgzMDM4MzEwZjMwMGQwNjAzNTUwNDA3MTMwNjVhNDE0NzUyNDU0MjMxMTQzMDEyMDYwMzU1MDQwMzBjMGI0ZDZmNmEyZDY1NTI2MWM0OGQ3NTZlMzA4MjAxMjIzMDBkMDYwOTJhODY0ODg2ZjcwZDAxMDEwMTA1MDAwMzgyMDEwZjAwMzA4MjAxMGEwMjgyMDEwMTAwYjkxMWQzMzBkMDYxYzg4NTdlMzc1NDJmOTcyNmFiMjZlMTUxM2QzMWE3OGUyYzIxYjVhZjAxMTMyOTEwYTVlZWU2OTg0OGIwZjE2M2YxZmQyZDlkZDgzYWQyNjgzODBkYzZiZjM2MWM5NDNjZTE2MGRjN2I0YjlhODJkOGI3ZTAxYTFhNTYyNjc0MTFkNTFhOTYyYzIwODE4NjlkOTgxZDkzZTM1M2UzMjQ4MWMyMzFlMGYyMDFiZWMxODZmZmFmZGMxMmEyNjQ0YzVhMjMwODU5NjhkN2Y3Njg2ZTU0NDllMjQyMjI5YzE4MTQ1YTE5MDI2OTY5NWFkYmRmNTk1YmQ1Y2FjNzFjOTU5OWUwY2Q1NTA1NTRhM2UyNjgzNDI0MjFiNGI2NTFkZDMwZmZmMzkwMmFkMDYxNDE2ZmQxNGZmNmRjNzJkZDM0Nzk5YTA0NDY1OWIxZjI3NzhlNGJmNDI2OThkMzg2MjUyZjZjZjkxMDU0Zjc3MmRmZWQ2NmQ0NDBjZDExZjJjNjVjOWNjOWY5N2Y3MGU2OTAzZTI0NmY2NzVjNDg4YWZmNThlY2Q5ZmZlOThmZTU1YzRmMWMwMDBkNGQ4OTJmOGE1ODE5ZDQ0YjgwNzk0MzI1NjNhMTkyZjMwYjVjZDc5ZGU4NmEzMTU4ZTlmNmQ4ZDBlNWY1ZmIwMjAzMDEwMDAxYTM4MjAxYjMzMDgyMDFhZjMwMGIwNjAzNTUxZDBmMDQwNDAzMDIwNWEwMzAzYzA2MDM1NTFkMjAwNDM1MzAzMzMwMzEwNjA5MmI3Yzg4NTAwNTBiMDUwMzAxMzAyNDMwMjIwNjA4MmIwNjAxMDUwNTA3MDIwMTE2MTY2ODc0NzQ3MDNhMmYyZjcyNjQ2MzJlNjY2OTZlNjEyZTY4NzIyZjYzNzAyZjMwMWUwNjAzNTUxZDExMDQxNzMwMTU4MTEzNmQ2MTcyNmI2ZjQwNmQ2ZjZhMmQ2NTcyNjE2Mzc1NmUyZTY4NzIzMDgxYzYwNjAzNTUxZDFmMDQ4MWJlMzA4MWJiMzA0M2EwNDFhMDNmYTQzZDMwM2IzMTBiMzAwOTA2MDM1NTA0MDYxMzAyNDg1MjMxMGQzMDBiMDYwMzU1MDQwYTEzMDQ0NjQ5NGU0MTMxMGMzMDBhMDYwMzU1MDQwYjEzMDM1MjQ0NDMzMTBmMzAwZDA2MDM1NTA0MDMxMzA2NDM1MjRjMzUzNDM5MzA3NGEwNzJhMDcwODY0ZDZjNjQ2MTcwM2EyZjJmNzI2NDYzMmQ2YzY0NjE3MDJlNjY2OTZlNjEyZTY4NzIyZjZmNzUzZDUyNDQ0MzJjNmYzZDQ2NDk0ZTQxMmM2MzNkNDg1MjNmNjM2NTcyNzQ2OTY2Njk2MzYxNzQ2NTUyNjU3NjZmNjM2MTc0Njk2ZjZlNGM2OTczNzQyNTMzNDI2MjY5NmU2MTcyNzk4NjFmNjg3NDc0NzAzYTJmMmY3MjY0NjMyZTY2Njk2ZTYxMmU2ODcyMmY2MzcyNmM3MzJmNzI2NDYzMmU2MzcyNmMzMDJiMDYwMzU1MWQxMDA0MjQzMDIyODAwZjMyMzAzMTM0MzAzMjMxMzIzMTM3MzIzNDMzMzA1YTgxMGYzMjMwMzEzOTMwMzIzMTMyMzEzNzM1MzQzMzMwNWEzMDEzMDYwMzU1MWQyMzA0MGMzMDBhODAwODQ3NDUwMDZlZjA1N2E2YzAzMDExMDYwMzU1MWQwZTA0MGEwNDA4NGM1MjkwNmMzNzY1ZjE0NjMwMDkwNjAzNTUxZDEzMDQwMjMwMDAzMDE5MDYwOTJhODY0ODg2ZjY3ZDA3NDEwMDA0MGMzMDBhMWIwNDU2MzgyZTMxMDMwMjAzYTgzMDBkMDYwOTJhODY0ODg2ZjcwZDAxMDEwNTA1MDAwMzgyMDEwMTAwY2JkZTZjMjRmMDY2MjhhYzAyOGQ1YTgzYzFjYzgyYTgwYWY3MDNkYmM0MGZlODE0YmRkMWU3NTQwMGQzOWZjZjJjZjhjYjNkYjUwOTg5ZWMzY2VjZDc0OTAxNmEyZmVkMzVjZDFiYWViZmQwNzcwYmU0ZmJjNDM5YjRkZGM4OTI3OWFjODQzMTI0NzljMDNjYTBjYjliODIyOGU2MzRiODYyZmNkNDdlMjRiNzNlOTk5YTliY2RhNWFjMDFkMDRkN2UwY2ZlODgwM2VmMjZlMjQ5NjhlYzBmMTUwNjljNmE1OWFmMmRiNTA4ODE2NTU5ODdkYmFhMmZiYzE5MDg0NWUzZWU2NmNjN2JkN2VhNGYyYmJiNDY0ODA2NjE1YjNkYTBlMGNlZWZjMzU5N2FlZDczMTliMDQyOWRjYTkyOTU0ODRkYmRmZDExYTk4MTE3NDJjMjEzY2UxMmVjZDg4YzcyNjNjZTM0ZjFlNTJhZThlNDA4ODMyNTQ4ODg4NjY0YjM0MzEyMTE0OWI2YjM0ZWYwN2IwMTRmN2E2Zjk0ZjMyZGViNjM0MjgxZGQzYzBjMDI4NDM3ZDRjMjdiZWYwY2NjZmNmYzM0MDE0Y2FmNTk0ZGE3OTdjMmJlYjlkMWMxNDkzNzQzZjc1MWYxODkxYzhhMWE2ZjhiMjhhZjM0NTMzMTgyMGRhZTMwODIwZGFhMDIwMTAxMzAzMjMwMmEzMTBiMzAwOTA2MDM1NTA0MDYxMzAyNDg1MjMxMGQzMDBiMDYwMzU1MDQwYTEzMDQ0NjQ5NGU0MTMxMGMzMDBhMDYwMzU1MDQwYjEzMDM1MjQ0NDMwMjA0M2YyYTU0ZjIzMDBkMDYwOTYwODY0ODAxNjUwMzA0MDIwMTA1MDBhMDRiMzAxODA2MDkyYTg2NDg4NmY3MGQwMTA5MDMzMTBiMDYwOTJhODY0ODg2ZjcwZDAxMDcwMTMwMmYwNjA5MmE4NjQ4ODZmNzBkMDEwOTA0MzEyMjA0MjAyZDVkZTAzMmNhM2FjODI3MDBiOGZmOTE1NGM2MThiMzI0MGRkZDIxNzc1MmQ0YzE0ZmU0ZTZkODYyNzBiOTQ4MzAwZDA2MDkyYTg2NDg4NmY3MGQwMTAxMDEwNTAwMDQ4MjAxMDAyNDgyNDMzZTI1Y2RjZmY0YWJjYTNkMzMyN2JhMDg4ZDdhYTk3OGM4MTcwNDFhYjk3NzU4NzYwNTRjYTViY2VmOWJkYmVmOTI1YjUxNjkxM2JiYjdlMTQ1YWZiNjM1MDFkOTJlNWI3NDJkNGFmMzNhMGUxOGNhMzA4MjAwNjIxMDMxM2FjMDEwZmE1NjVlNDkwNjBjNjgxZjMzNjFjM2Q5NjZhYWEwMjYyZjYzMmU3NDExZjVhNzA2NTY2NGIxOTExM2IzZjMzMzdkMzkzYzRkN2FkNDkwN2JiMDllN2M1YmNkYWU5NjVhMGIzNDliZmU0M2MwNWEwOTFkNmM1YzA4MWEwZTg5ZjVkNjUyNDkxYjdjM2UzNmVlYzg4N2ZjOWE5M2Y3OWE5M2QwOGJmMmUxN2ZhNjUwYTg5MGFlZTQ2YTk3MjFhZDNiZGIzYjIwYzk5ODIzODZkMmUxOWUxZmVhMmFhNGE0YTQ4YTM5MGU4ZDhkYTE0ODVlMDYzOTg3YWM2NjgzZmZkN2RhMDdjOWU4OTBiODE5YWQ3YWYwMDg3NjNkOGJkNGQyNjAzNzZiMzc2YTJiZjVkZDJlMGIyOTBhYjE1NmI3Yzc3Y2Q0NjVlMWQ4NjllMDcwNWFhM2IwZDAwMmJiMjFiNWNjY2UyYjQzZmQ2MzA1MDI3NWRjMTk0ZWExODIwYzAwMzA4MjBiZmMwNjBiMmE4NjQ4ODZmNzBkMDEwOTEwMDIwZTMxODIwYmViMzA4MjBiZTcwNjA5MmE4NjQ4ODZmNzBkMDEwNzAyYTA4MjBiZDgzMDgyMGJkNDAyMDEwMzMxMGYzMDBkMDYwOTYwODY0ODAxNjUwMzA0MDIwMTA1MDAzMDgyMDEzMTA2MGIyYTg2NDg4NmY3MGQwMTA5MTAwMTA0YTA4MjAxMjAwNDgyMDExYzMwODIwMTE4MDIwMTAxMDYwZTJiMDYwMTA0MDE4MWE4MTgwMjAxMDE1NjAyMDIzMDMxMzAwZDA2MDk2MDg2NDgwMTY1MDMwNDAyMDEwNTAwMDQyMDViOGYxMDc4OWYzN2YxYmI2ZjFkNzBmYTk2ZmNiNTdjYmQzNzQ2Y2RiYWM5YTBkN2UwMGFmNTMwZjE5NzBmMzYwMjA1MDQwMGZlYmFkOTE4MGYzMjMwMzEzNzMwMzkzMjMxMzEzMjMwMzEzMTMxNWEzMDAzMDIwMTAxMDIwODA4ZDUwMGY5NGRiZmEwYjFhMDgxOGJhNDgxODgzMDgxODUzMTBiMzAwOTA2MDM1NTA0MDYxMzAyNDg1NTMxMTEzMDBmMDYwMzU1MDQwNzBjMDg0Mjc1NjQ2MTcwNjU3Mzc0MzExNjMwMTQwNjAzNTUwNDBhMGMwZDRkNjk2MzcyNmY3MzY1NjMyMDRjNzQ2NDJlMzExYzMwMWEwNjAzNTUwNDYxMGMxMzU2NDE1NDQ4NTUyZDMyMzMzNTM4MzQzNDM5MzcyZDMyMmQzNDMxMzEyZDMwMmIwNjAzNTUwNDAzMGMyNDUxNzU2MTZjNjk2NjY5NjU2NDIwNjU0OTQ0NDE1MzIwNjUyZDUzN2E2OTY3NmU2ZjIwNTQ1MzQxMjAzMjMwMzEzNzIwMzAzMWExMWIzMDE5MDYwODJiMDYwMTA1MDUwNzAxMDMwNDBkMzAwYjMwMDkwNjA3MDQwMDgxOTc1ZTAxMDFhMDgyMDgyODMwODIwODI0MzA4MjA3MGNhMDAzMDIwMTAyMDIwZDAwOTUwMzI5ZmNjYjY5ZTgxOTM4MjE1YjBhMzAwZDA2MDkyYTg2NDg4NmY3MGQwMTAxMGIwNTAwMzA4MTgyMzEwYjMwMDkwNjAzNTUwNDA2MTMwMjQ4NTUzMTExMzAwZjA2MDM1NTA0MDcwYzA4NDI3NTY0NjE3MDY1NzM3NDMxMTYzMDE0MDYwMzU1MDQwYTBjMGQ0ZDY5NjM3MjZmNzM2NTYzMjA0Yzc0NjQyZTMxMjczMDI1MDYwMzU1MDQwMzBjMWU0ZDY5NjM3MjZmNzM2NTYzMjA2NTJkNTM3YTY5Njc2ZTZmMjA1MjZmNmY3NDIwNDM0MTIwMzIzMDMwMzkzMTFmMzAxZDA2MDkyYTg2NDg4NmY3MGQwMTA5MDExNjEwNjk2ZTY2NmY0MDY1MmQ3MzdhNjk2NzZlNmYyZTY4NzUzMDFlMTcwZDMxMzczMDMyMzEzNzMwMzAzMDMwMzAzMDVhMTcwZDMyMzkzMDMyMzEzNzMwMzAzMDMwMzAzMDVhMzA4MTg1MzEwYjMwMDkwNjAzNTUwNDA2MTMwMjQ4NTUzMTExMzAwZjA2MDM1NTA0MDcwYzA4NDI3NTY0NjE3MDY1NzM3NDMxMTYzMDE0MDYwMzU1MDQwYTBjMGQ0ZDY5NjM3MjZmNzM2NTYzMjA0Yzc0NjQyZTMxMWMzMDFhMDYwMzU1MDQ2MTBjMTM1NjQxNTQ0ODU1MmQzMjMzMzUzODM0MzQzOTM3MmQzMjJkMzQzMTMxMmQzMDJiMDYwMzU1MDQwMzBjMjQ1MTc1NjE2YzY5NjY2OTY1NjQyMDY1NDk0NDQxNTMyMDY1MmQ1MzdhNjk2NzZlNmYyMDU0NTM0MTIwMzIzMDMxMzcyMDMwMzEzMDgyMDEyMjMwMGQwNjA5MmE4NjQ4ODZmNzBkMDEwMTAxMDUwMDAzODIwMTBmMDAzMDgyMDEwYTAyODIwMTAxMDBhMmIxOWI4ZTcyYWFhMDU3OTY3MjUyMmI0YmJjYTY4ZDhiYzY2YmMyZTY0NTZmZDhjODYyMGUzMGVkOTRkNTVhODVlZjA5OTExMzJjMzI5Y2M5ZDQ4OGZjM2NlNWUxODgyNDc3MmUwYjA0NzhiYWZhMTNlOTAwZTlkMDFmZWZjNTAxMjYzZTRmZjE1MWYyNWE5NTBkNDBmNzhiYWZmNWRiYjM1MTJlZjkyOTZkYTAyZTI4NTE0MWMyZjM2NzM3MmE5NTY3YWNkM2E0ZTQ0NTExNDdhNjRkZTIxYmMxMTA5YmU4M2E0NzU2ZDI0YmI3YTVmODQ4NTYwNTI4ZDU0NzE3YzIwMGQ4OTZiN2EzY2M0OWExNTU3ZTUzNjk3MTRiMzYwMzAxM2RlZGJkNTJkODE1ZDZhODdiZDQ1MDY5MmM0OTU3YTgxY2FhNjY2YjI4MjRjYzIwYTcwMmYxNTJiZWFlMmU0YjMwNWVhYjU4MmFiNjg3YzU4NjhiYmFjYjZkZjZjYzBkYWFhODUxMWRjZmIwYmEyZDY0MjY1NDhlOGEyZGE4OGZiYTJmYWZjODVmZDY4MjZhYzdhM2RlYTRmNWM1NTQ5OTE5YTM3NjhhOTg0MjZkYjM2MWVlYmM1YTI0YzMxNGRkZjUwM2E5OWIxZTA4YTE5N2JjYjdhOTFkYjI1NzAyMDMwMTAwMDFhMzgyMDQ5MjMwODIwNDhlMzAwZTA2MDM1NTFkMGYwMTAxZmYwNDA0MDMwMjA2YzAzMDE2MDYwMzU1MWQyNTAxMDFmZjA0MGMzMDBhMDYwODJiMDYwMTA1MDUwNzAzMDgzMDgyMDEyZTA2MDM1NTFkMjAwNDgyMDEyNTMwODIwMTIxMzA4MjAxMWQwNjBlMmIwNjAxMDQwMTgxYTgxODAyMDEwMTUyMDIwMjMwODIwMTA5MzAyNjA2MDgyYjA2MDEwNTA1MDcwMjAxMTYxYTY4NzQ3NDcwM2EyZjJmNjM3MDJlNjUyZDczN2E2OTY3NmU2ZjJlNjg3NTJmNzE2MzcwNzMzMDU0MDYwODJiMDYwMTA1MDUwNzAyMDIzMDQ4MWE0NjRjNjk2ZDY5NzQ2MTc0Njk2ZjZlMjA2ZjY2MjA2NjY5NmU2MTZlNjM2OTYxNmMyMDZjNjk2MTYyNjk2YzY5NzQ3OTNhMjAzMTMwMzAyYzMwMzAzMDIwNDg1NTQ2MjA3MDY1NzIyMDY5NmU3Mzc1NzI2MTZlNjM2NTIwNjk2ZTYzNjk2NDY1NmU3NDJlMzA4MTg4MDYwODJiMDYwMTA1MDUwNzAyMDIzMDdjMWU3YTAwNTAwMGU5MDA2ZTAwN2EwMGZjMDA2NzAwNzkwMDY5MDAyMDAwNjYwMDY1MDA2YzAwNjUwMDZjMDE1MTAwNzMwMDczMDBlOTAwNjcwMDIwMDA2YjAwNmYwMDcyMDA2YzAwZTEwMDc0MDA2ZjAwN2EwMGUxMDA3MzAwNjEwMDNhMDAyMDAwMzEwMDMwMDAzMDAwMjAwMDMwMDAzMDAwMzAwMDIwMDA0NjAwNzQwMDIwMDA2YjAwZTEwMDcyMDA2NTAwNzMwMDY1MDA2ZDAwZTkwMDZlMDA3OTAwNjUwMDZlMDA2YjAwZTkwMDZlMDA3NDAwMmUzMDFkMDYwMzU1MWQwZTA0MTYwNDE0Njg1MmE0OGMzNDAwNjYwM2IxNWM0YmQ3M2Y3MjdkNGMxMDQ0OWJmODMwMWYwNjAzNTUxZDIzMDQxODMwMTY4MDE0Y2IwZmM2ZGY0MjQzY2MzZGNiYjU0ODIzYTExYTdhYTYyYWJiMzQ2ODMwODFiNjA2MDM1NTFkMWYwNDgxYWUzMDgxYWIzMDM3YTAzNWEwMzM4NjMxNjg3NDc0NzAzYTJmMmY3MjZmNmY3NDYzNjEzMjMwMzAzOTJkNjM3MjZjMzEyZTY1MmQ3MzdhNjk2NzZlNmYyZTY4NzUyZjcyNmY2Zjc0NjM2MTMyMzAzMDM5MmU2MzcyNmMzMDM3YTAzNWEwMzM4NjMxNjg3NDc0NzAzYTJmMmY3MjZmNmY3NDYzNjEzMjMwMzAzOTJkNjM3MjZjMzIyZTY1MmQ3MzdhNjk2NzZlNmYyZTY4NzUyZjcyNmY2Zjc0NjM2MTMyMzAzMDM5MmU2MzcyNmMzMDM3YTAzNWEwMzM4NjMxNjg3NDc0NzAzYTJmMmY3MjZmNmY3NDYzNjEzMjMwMzAzOTJkNjM3MjZjMzMyZTY1MmQ3MzdhNjk2NzZlNmYyZTY4NzUyZjcyNmY2Zjc0NjM2MTMyMzAzMDM5MmU2MzcyNmMzMDgyMDE1ZjA2MDgyYjA2MDEwNTA1MDcwMTAxMDQ4MjAxNTEzMDgyMDE0ZDMwMmYwNjA4MmIwNjAxMDUwNTA3MzAwMTg2MjM2ODc0NzQ3MDNhMmYyZjcyNmY2Zjc0NjM2MTMyMzAzMDM5MmQ2ZjYzNzM3MDMxMmU2NTJkNzM3YTY5Njc2ZTZmMmU2ODc1MzAyZjA2MDgyYjA2MDEwNTA1MDczMDAxODYyMzY4NzQ3NDcwM2EyZjJmNzI2ZjZmNzQ2MzYxMzIzMDMwMzkyZDZmNjM3MzcwMzIyZTY1MmQ3MzdhNjk2NzZlNmYyZTY4NzUzMDJmMDYwODJiMDYwMTA1MDUwNzMwMDE4NjIzNjg3NDc0NzAzYTJmMmY3MjZmNmY3NDYzNjEzMjMwMzAzOTJkNmY2MzczNzAzMzJlNjUyZDczN2E2OTY3NmU2ZjJlNjg3NTMwM2MwNjA4MmIwNjAxMDUwNTA3MzAwMjg2MzA2ODc0NzQ3MDNhMmYyZjcyNmY2Zjc0NjM2MTMyMzAzMDM5MmQ2MzYxMzEyZTY1MmQ3MzdhNjk2NzZlNmYyZTY4NzUyZjcyNmY2Zjc0NjM2MTMyMzAzMDM5MmU2MzcyNzQzMDNjMDYwODJiMDYwMTA1MDUwNzMwMDI4NjMwNjg3NDc0NzAzYTJmMmY3MjZmNmY3NDYzNjEzMjMwMzAzOTJkNjM2MTMyMmU2NTJkNzM3YTY5Njc2ZTZmMmU2ODc1MmY3MjZmNmY3NDYzNjEzMjMwMzAzOTJlNjM3Mjc0MzAzYzA2MDgyYjA2MDEwNTA1MDczMDAyODYzMDY4NzQ3NDcwM2EyZjJmNzI2ZjZmNzQ2MzYxMzIzMDMwMzkyZDYzNjEzMzJlNjUyZDczN2E2OTY3NmU2ZjJlNjg3NTJmNzI2ZjZmNzQ2MzYxMzIzMDMwMzkyZTYzNzI3NDMwMmIwNjAzNTUxZDEwMDQyNDMwMjI4MDBmMzIzMDMxMzczMDMyMzEzNzMwMzAzMDMwMzAzMDVhODEwZjMyMzAzMTM4MzAzODMxMzczMDMwMzAzMDMwMzA1YTMwODFhODA2MDgyYjA2MDEwNTA1MDcwMTAzMDQ4MTliMzA4MTk4MzAwODA2MDYwNDAwOGU0NjAxMDEzMDE1MDYwNjA0MDA4ZTQ2MDEwMjMwMGIxMzAzNDg1NTQ2MDIwMTAxMDIwMTA1MzAwYjA2MDYwNDAwOGU0NjAxMDMwMjAxMGEzMDUzMDYwNjA0MDA4ZTQ2MDEwNTMwNDkzMDI0MTYxZTY4NzQ3NDcwNzMzYTJmMmY2MzcwMmU2NTJkNzM3YTY5Njc2ZTZmMmU2ODc1MmY3MTYzNzA3MzVmNjU2ZTEzMDI0NTRlMzAyMTE2MWI2ODc0NzQ3MDczM2EyZjJmNjM3MDJlNjUyZDczN2E2OTY3NmU2ZjJlNjg3NTJmNzE2MzcwNzMxMzAyNDg1NTMwMTMwNjA2MDQwMDhlNDYwMTA2MzAwOTA2MDcwNDAwOGU0NjAxMDYwMjMwMGQwNjA5MmE4NjQ4ODZmNzBkMDEwMTBiMDUwMDAzODIwMTAxMDBhYjU2NTVmMDI3MzllNzA2Y2I5MDJjZDUwMjQ2YTcwM2MxYjIzNzlkNDYwOTA4YWQ1OWFjYzI3YTU1ZThjOWFiOTMyODZiZTIzZGIyMGVhYTRiNmNmY2NlNjFkZTZlYThlZjFhMGNhMzIwM2I3NGJiNjU2MTM0OWUzM2MxZjE4ZDk2NDc0NTk2OTI5ODkwY2M3ZDM0MmZlNjc4MTIyY2ViZTE5MjAzZmExN2NiZTNiMzcxOWNkZjE2OTM2NGZkZTA0YjEzZjg0Y2MwMDg0OTZlMzMyYTNjNDU3N2NlMzcxMTdmNjYwMWQyYTcwNmFiZTM5NWFmMTJiMWJiMTQyNTQyZmQ0Yjk0ODQ1MTNhYjllYzJmNGRhODE3ZWIzMGQyMjVlY2RlOGQ4YTYwZjliYjEyY2FlZTk1NzIxZmU2MTU2ZDNkZDc1YWJjZjQ3YzUyMDZmNWVlODVjNDIyYjM2ZjAyMjhmYjMwMTFmYzQwYjg4NTVkOTI3NGYyNGI3ZjBlNzgxNjAwYTQ2NmVjMDQyOWU0ZDcwZjdjNzg2ZjYzNjMzY2JkMGUyYWRjZWU1Y2UzZWZmMmM4MTEzMTE1M2M5OWI3OWY0ODliYTA0ZjcwMGQ3Yjg2ZTc1NGQxZTU0YzZkOGNhYzM3ZGRiM2E3YzhmZWNkNGIwNjBkNGMzZWE3OGI4YTMxODIwMjViMzA4MjAyNTcwMjAxMDEzMDgxOTQzMDgxODIzMTBiMzAwOTA2MDM1NTA0MDYxMzAyNDg1NTMxMTEzMDBmMDYwMzU1MDQwNzBjMDg0Mjc1NjQ2MTcwNjU3Mzc0MzExNjMwMTQwNjAzNTUwNDBhMGMwZDRkNjk2MzcyNmY3MzY1NjMyMDRjNzQ2NDJlMzEyNzMwMjUwNjAzNTUwNDAzMGMxZTRkNjk2MzcyNmY3MzY1NjMyMDY1MmQ1MzdhNjk2NzZlNmYyMDUyNmY2Zjc0MjA0MzQxMjAzMjMwMzAzOTMxMWYzMDFkMDYwOTJhODY0ODg2ZjcwZDAxMDkwMTE2MTA2OTZlNjY2ZjQwNjUyZDczN2E2OTY3NmU2ZjJlNjg3NTAyMGQwMDk1MDMyOWZjY2I2OWU4MTkzODIxNWIwYTMwMGQwNjA5NjA4NjQ4MDE2NTAzMDQwMjAxMDUwMGEwODE5ODMwMWEwNjA5MmE4NjQ4ODZmNzBkMDEwOTAzMzEwZDA2MGIyYTg2NDg4NmY3MGQwMTA5MTAwMTA0MzAxYzA2MDkyYTg2NDg4NmY3MGQwMTA5MDUzMTBmMTcwZDMxMzczMDM5MzIzMTMxMzIzMDMxMzEzMTVhMzAyYjA2MGIyYTg2NDg4NmY3MGQwMTA5MTAwMjBjMzExYzMwMWEzMDE4MzAxNjA0MTQ3YTYxOGU4NjViODFlMTU2ODYwMGY5N2EwYmMwZGM4MmE0NTc0MWY1MzAyZjA2MDkyYTg2NDg4NmY3MGQwMTA5MDQzMTIyMDQyMGZkYmUwNDE2ZTM1MjQyMTBhZTVhMGUxOTVlODNiOTI3YjI4MTliZGUyNzhiMTk0YzQxZGI1OWI1NmVkOGMwZWQzMDBkMDYwOTJhODY0ODg2ZjcwZDAxMDEwMTA1MDAwNDgyMDEwMDFiYjJkOWVkMGQ5MWViZjFhMDU1OWNlZDJlYTNjMjE5YjI1YmUxNTUxMGRjZWM5NTQ2MDE1YTc4NzNlMzdmOWE0MjQ4YTBjNzI0Y2M0MDc2YWFiMGQyMTgxZDZjZGUxMjMwOWVhZGM3NzNkY2RmZmQ1YTY1ZmE0NGMzZDYwYjllZjkwMTU5MWVmMzQwNjIyZDA5Njc1ODJkZTcxYWRjYjI0NzE0OThjNjBmN2IxNDdiOGU5NzU2YjZkY2VmODNkNDcyN2UxYWJiN2VkYTdiYzI3ODIzNmY0NzkxMjMxMzY2NGY0ODI3ZjMxNjA4NzNkMjRmMTk3ZDBjM2E5OGZkM2Y1ZDgxOTczNDBmZWI4M2QyOTE4ODEzYzdkNmVmNjhlMWUwYTUyMjEyYzBlNjZlMWI0YzhlNDEwNzZmY2JjYTY4YTIyZGM1OTNhOTJkYzQ2ZDA0NTU2MjJmNzRjYTQxNTFmZDZkMWE1ZjBjZjUwMGQ2N2M5YTY0OGJkM2I5NmI2ODFiOTFlYzM3MDZmNzA2NzJjNWRkOTdiNTk4NDgxZDI1YTM3NGVjNzc0MzE0NTIzYjg1OWVkZGUwOWM1NTY4Nzg5MDYyNDg2NmQ5YTVhMGExZjNjNzE2ODYxZTU3MWQwNmM4YWNiMjU3MDRiYTMxZDAyNzk1YjMzNjNmOTFhODdkMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMD4+PgplbmRvYmoKMTggMCBvYmoKPDwvVHlwZS9Gb250L0Jhc2VGb250L0hlbHZldGljYS9FbmNvZGluZy9XaW5BbnNpRW5jb2RpbmcvTmFtZS9IZWx2L1N1YnR5cGUvVHlwZTE+PgplbmRvYmoKMTkgMCBvYmoKPDwvVHlwZS9Gb250L0Jhc2VGb250L1phcGZEaW5nYmF0cy9OYW1lL1phRGIvU3VidHlwZS9UeXBlMT4+CmVuZG9iagoxNiAwIG9iago8PC9UeXBlL1hPYmplY3QvU3VidHlwZS9Gb3JtL1Jlc291cmNlczw8Pj4vQkJveFswIDAgMCAwXS9Gb3JtVHlwZSAxL01hdHJpeCBbMSAwIDAgMSAwIDBdL0xlbmd0aCA4L0ZpbHRlci9GbGF0ZURlY29kZT4+c3RyZWFtCnicAwAAAAABCmVuZHN0cmVhbQplbmRvYmoKMTQgMCBvYmoKPDwvUHJvZHVjZXIoaVRleHRTaGFycJIgNS41LjExIKkyMDAwLTIwMTcgaVRleHQgR3JvdXAgTlYgXChBR1BMLXZlcnNpb25cKTsgbW9kaWZpZWQgdXNpbmcgaVRleHRTaGFycJIgNS41LjExIKkyMDAwLTIwMTcgaVRleHQgR3JvdXAgTlYgXChBR1BMLXZlcnNpb25cKSkvQ3JlYXRpb25EYXRlKEQ6MjAxNzA5MjExNDAxMTMrMDInMDAnKS9Nb2REYXRlKEQ6MjAxNzA5MjExNDAxMTMrMDInMDAnKT4+CmVuZG9iagoxMyAwIG9iago8PC9UeXBlL0NhdGFsb2cvUGFnZXMgNSAwIFIvT0NQcm9wZXJ0aWVzPDwvT0NHc1sxIDAgUl0vRDw8L09yZGVyWzEgMCBSXS9OYW1lKERpc2NsYWltZXIgXChOTiAxNTAvMDVcKSkvQVNbPDwvRXZlbnQvVmlldy9DYXRlZ29yeVsvVmlld10vT0NHc1sxIDAgUl0+Pjw8L0V2ZW50L1ByaW50L0NhdGVnb3J5Wy9QcmludF0vT0NHc1sxIDAgUl0+Pl0vTGlzdE1vZGUvVmlzaWJsZVBhZ2VzPj4+Pi9BY3JvRm9ybTw8L0ZpZWxkc1sxNyAwIFJdL0RBKC9IZWx2IDAgVGYgMCBnICkvRFI8PC9Gb250PDwvSGVsdiAxOCAwIFIvWmFEYiAxOSAwIFI+Pj4+L1NpZ0ZsYWdzIDM+Pi9WZXJzaW9uLzEuNz4+CmVuZG9iago2IDAgb2JqCjw8L1R5cGUvUGFnZS9NZWRpYUJveFswIDAgNTk1LjIyIDg0Ml0vUmVzb3VyY2VzPDwvRm9udDw8L0YxIDMgMCBSPj4vWE9iamVjdDw8L1hmMSAyIDAgUj4+L1Byb3BlcnRpZXM8PC9QcjEgMSAwIFI+Pj4+L0NvbnRlbnRzIDQgMCBSL1BhcmVudCA1IDAgUi9Bbm5vdHNbMTcgMCBSXT4+CmVuZG9iagp4cmVmCjAgMQowMDAwMDAwMDAwIDY1NTM1IGYgCjYgMQowMDAwMDM4NTU4IDAwMDAwIG4gCjEzIDcKMDAwMDAzODIwNyAwMDAwMCBuIAowMDAwMDM3OTY1IDAwMDAwIG4gCjAwMDAwMTIwMzQgMDAwMDAgbiAKMDAwMDAzNzgwNSAwMDAwMCBuIAowMDAwMDExOTA2IDAwMDAwIG4gCjAwMDAwMzc2MjkgMDAwMDAgbiAKMDAwMDAzNzcyOCAwMDAwMCBuIAp0cmFpbGVyCjw8L1NpemUgMjAvUm9vdCAxMyAwIFIvSW5mbyAxNCAwIFIvSUQgWzwwNWNjMTA3Y2ZkMjg0NTBjZjExNWE2ZjU1NWQzMjZiZT48MDQwMTJkZTUwODVkYjk4ZWRlN2NlYzViNjY5ZjE5NWU+XS9QcmV2IDExNDQwPj4KJWlUZXh0LTUuNS4xMQpzdGFydHhyZWYKMzg3MzUKJSVFT0YK</cbc:EmbeddedDocumentBinaryObject>
                        </cac:Attachment>
                        </AttachedDocument>
                    </AttachedDocumentEnvelope>
                </OutgoingInvoice>
            </OutgoingInvoicesData>";
            return xmlString.Replace(Environment.NewLine, "").Replace("                        ", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fakturaStavke"></param>
        /// <returns></returns>
        private string GetInvoiceLines(List<FakturaStavka> fakturaStavke, bool sustavPdv)
        {
            string result = "";
            TaxTotal taxTotal = GetTaxTotal(sustavPdv);
            if (fakturaStavke.Count > 0)
            {
                int id = 1;
                decimal ukupnoBezPdv = 0;
                decimal ukupnoPdv = 0;
                foreach (FakturaStavka stavka in fakturaStavke)
                {
                    decimal pdvIznos = stavka.IznosBezPdv * (stavka.Pdv / 100);
                    ukupnoPdv += pdvIznos;
                    ukupnoBezPdv += stavka.IznosBezPdv;
                    result += $@"<cac:InvoiceLine>
                                    <cbc:ID>{id}</cbc:ID>
                                    <cbc:InvoicedQuantity unitCode='MTQ' unitCodeListID='UN/ECE rec 20 8e' unitCodeListAgencyID='6'>{stavka.Kolicina.ToString().Replace(',', '.')}</cbc:InvoicedQuantity>
                                    <cbc:LineExtensionAmount currencyID='HRK'>{stavka.IznosBezPdv.ToString().Replace(',', '.')}</cbc:LineExtensionAmount>
                                    <cac:AllowanceCharge>
                                        <cbc:ChargeIndicator>false</cbc:ChargeIndicator>
                                        <cbc:MultiplierFactorNumeric>0.5</cbc:MultiplierFactorNumeric>
                                        <cbc:Amount currencyID='HRK'>200.0</cbc:Amount>
                                        <cbc:BaseAmount currencyID='HRK'>400</cbc:BaseAmount>
                                    </cac:AllowanceCharge>
                                    <cac:TaxTotal>
                                    <cbc:TaxAmount currencyID='HRK'>{pdvIznos.ToString().Replace(',', '.')}</cbc:TaxAmount>
                                    <cac:TaxSubtotal>
                                        <cbc:TaxableAmount currencyID='HRK'>{stavka.IznosBezPdv.ToString().Replace(',', '.')}</cbc:TaxableAmount>
                                        <cbc:TaxAmount currencyID='HRK'>{pdvIznos.ToString().Replace(',', '.')}</cbc:TaxAmount>
                                        <cac:TaxCategory>
                                        <cbc:ID schemeID='UN/ECE 5305' schemeAgencyID='6' schemeURI='http://www.unece.org/trade/untdid/d07a/tred/tred5305.htm'>{taxTotal.Category.ID}</cbc:ID>
                                        <cbc:Name>{taxTotal.Category.Name}</cbc:Name>
                                        <cbc:Percent>{stavka.Pdv.ToString().Replace(',', '.')}</cbc:Percent>
                                        {(!sustavPdv ? "<cbc:TaxExemptionReason>Oslobođeno od poreza prema članku zakona</cbc:TaxExemptionReason>" : "")}
                                        <cac:TaxScheme>
                                            <cbc:Name>{taxTotal.Scheme.Name}</cbc:Name>
                                            <cbc:TaxTypeCode>{taxTotal.Scheme.TaxTypeCode}</cbc:TaxTypeCode>
                                        </cac:TaxScheme>
                                        </cac:TaxCategory>
                                    </cac:TaxSubtotal>
                                    </cac:TaxTotal>
                                    <cac:Item>
                                    <cbc:Name>{stavka.Naziv}</cbc:Name>
                                    <cac:AdditionalItemIdentification>
                                        <cbc:ID schemeID='GTIN'>3851987654321</cbc:ID>
                                    </cac:AdditionalItemIdentification>
                                    </cac:Item>
                                    <cac:Price>
                                        <cbc:PriceAmount currencyID='HRK'>{stavka.Cijena.ToString().Replace(',', '.')}</cbc:PriceAmount>
                                        <cbc:BaseQuantity unitCode='H87'>1</cbc:BaseQuantity>
                                    </cac:Price>
                                </cac:InvoiceLine>{Environment.NewLine}";
                    id++;
                }
                string payment = $@"<cac:PaymentTerms>
                                <cbc:PrepaidPaymentReferenceID>1223-1-2</cbc:PrepaidPaymentReferenceID>
                                <cbc:Note>Uvjeti plaćanja...</cbc:Note>
                                <cbc:Amount currencyID='HRK'>{(ukupnoBezPdv + ukupnoPdv).ToString().Replace(',', '.')}</cbc:Amount>
                            </cac:PaymentTerms>
                                <cac:TaxTotal>
                                <cbc:TaxAmount currencyID='HRK'>{ukupnoPdv.ToString().Replace(',', '.')}</cbc:TaxAmount>
                                <cac:TaxSubtotal>
                                <cbc:TaxableAmount currencyID='HRK'>{ukupnoBezPdv.ToString().Replace(',', '.')}</cbc:TaxableAmount>
                                <cbc:TaxAmount currencyID='HRK'>{ukupnoPdv.ToString().Replace(',', '.')}</cbc:TaxAmount>
                                <cac:TaxCategory>
                                    <cbc:ID schemeID='UN/ECE 5305' schemeAgencyID='6' schemeURI='http://www.unece.org/trade/untdid/d07a/tred/tred5305.htm'>{taxTotal.Category.ID}</cbc:ID>
                                    <cbc:Name>{taxTotal.Category.Name}</cbc:Name>
                                    <cbc:Percent>{taxTotal.Category.Percent}</cbc:Percent>
                                    {(!sustavPdv ? "<cbc:TaxExemptionReason>Oslobođeno od poreza prema članku zakona</cbc:TaxExemptionReason>" : "")}
                                    <cac:TaxScheme>
                                    <cbc:Name>{taxTotal.Scheme.Name}</cbc:Name>
                                    <cbc:TaxTypeCode>{taxTotal.Scheme.TaxTypeCode}</cbc:TaxTypeCode>
                                    </cac:TaxScheme>
                                </cac:TaxCategory>
                                </cac:TaxSubtotal>
                            </cac:TaxTotal>
                            <cac:LegalMonetaryTotal>
                                <cbc:LineExtensionAmount currencyID='HRK'>{ukupnoBezPdv.ToString().Replace(',', '.')}</cbc:LineExtensionAmount>
                                <cbc:TaxExclusiveAmount currencyID='HRK'>{ukupnoBezPdv.ToString().Replace(',', '.')}</cbc:TaxExclusiveAmount>
                                <cbc:TaxInclusiveAmount currencyID='HRK'>{(ukupnoBezPdv + ukupnoPdv).ToString().Replace(',', '.')}</cbc:TaxInclusiveAmount>
                                <cbc:AllowanceTotalAmount currencyID='HRK'>200.0</cbc:AllowanceTotalAmount>
                                <cbc:PayableAmount currencyID='HRK'>{(ukupnoBezPdv + ukupnoPdv).ToString().Replace(',', '.')}</cbc:PayableAmount>
                            </cac:LegalMonetaryTotal>";
                result = payment + result;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tax"></param>
        /// <returns></returns>
        private TaxTotal GetTaxTotal(bool tax)
        {
            TaxTotal.TaxCategory taxCategory;
            TaxTotal.TaxScheme taxScheme;

            if (tax)
            {
                taxCategory = new TaxTotal.TaxCategory
                {
                    ID = "S",
                    Name = "PDV",
                    Percent = 25
                };

                taxScheme = new TaxTotal.TaxScheme
                {
                    Name = "VAT",
                    TaxTypeCode = Enums.TaxTypeCode.StandardRated
                };
            }
            else
            {
                taxCategory = new TaxTotal.TaxCategory
                {
                    ID = "E",
                    Name = "OSLOBOĐENO_POREZA",
                    Percent = 0
                };

                taxScheme = new TaxTotal.TaxScheme
                {
                    Name = "FRE",
                    TaxTypeCode = Enums.TaxTypeCode.ZeroRated
                };
            }

            return (new TaxTotal { Category = taxCategory, Scheme = taxScheme });
        }
    }
}
