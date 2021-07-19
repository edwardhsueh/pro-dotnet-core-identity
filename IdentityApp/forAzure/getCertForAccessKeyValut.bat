REM Read https://docs.microsoft.com/zh-tw/azure/vpn-gateway/vpn-gateway-certificates-point-to-site
REM run powerShell as administrator
REM P2SRootCert
call $cert = New-SelfSignedCertificate -Type Custom -KeySpec Signature `
-Subject "CN=P2SRootCert" -KeyExportPolicy Exportable `
-HashAlgorithm sha256 -KeyLength 2048 `
-CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage CertSign
REM P2SChildCert for local usage
call New-SelfSignedCertificate -Type Custom -DnsName P2SChildCert -KeySpec Signature `
-Subject "CN=P2SChildCert" -KeyExportPolicy Exportable `
-HashAlgorithm sha256 -KeyLength 2048 `
-CertStoreLocation "Cert:\CurrentUser\My" `
-Signer $cert -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.2")
REM export Cert
REM 若要取得憑證的 .cer 檔案，請開啟 [管理使用者憑證]。 找出自我簽署的根憑證，通常位於 '[憑證 - 目前的使用者][個人][憑證]' 中，然後按一下滑鼠右鍵。 按一下 [所有工作]，然後按一下 [匯出]。 這會開啟 [憑證匯出精靈] 。 若您在 Current User\Personal\Certificates 下找不到憑證，您可能已意外開啟 [憑證 - 本機電腦]，而非 [憑證 - 目前使用者]。 若要使用 PowerShell 在目前使用者範圍開啟 [憑證管理員]，您必須在主控台視窗中輸入 certmgr。
REM　using Azure AD to create Application and upload certificate from previous step
REM in AZure Key Valut, set access policy for prevoius App
REM in appSetting.json, set related setting

