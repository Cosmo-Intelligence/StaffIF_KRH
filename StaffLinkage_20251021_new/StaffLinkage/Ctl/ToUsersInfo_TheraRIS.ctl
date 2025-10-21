OPTIONS(SKIP = 1, ERRORS = 999999, ROWS = 1000000)
LOAD DATA CHARACTERSET JA16SJIS
APPEND
INTO TABLE TOUSERSINFO
FIELDS
TERMINATED BY "," OPTIONALLY ENCLOSED BY '"' TRAILING NULLCOLS
(
REQUESTID "to_char(sysdate, 'DD') || lpad(ss_toUsersInfoRequestid.nextval, 6, '0')",
REQUESTDATE "(select sysdate from dual)",
DB "(select 'THERARIS' from dual)",
APPCODE "(select 'THERARIS' from dual)",
USERID,
HOSPITALID "(select 'HID' from dual)",
PASSWORD ":USERID",
USERNAMEKANJI,
USERNAMEENG,
SECTION_ID,
TANTO_SECTION_ID,
STAFFID,
SYOKUIN_KBN,
TEL,
PASSWORDEXPIRYDATE "(select case when :PASSWORDEXPIRYDATE > '21001231' then to_date('20991231') else to_date(:PASSWORDEXPIRYDATE) end from dual)",
PASSWORDWARNINGDATE "(select case when :PASSWORDEXPIRYDATE > '21001231' then add_months(to_date('20991231'), -1) else add_months(to_date(:PASSWORDEXPIRYDATE), -1) end from dual)",
USERIDVALIDITYFLAG,
REQUESTTYPE,
MESSAGEID1 "(select '' from dual)",
MESSAGEID2 "(select '' from dual)",
MESSAGEID3 "(select '' from dual)",
TRANSFERSTATUS "(select '00' from dual)",
TRANSFERDATE "(select '' from dual)",
TRANSFERRESULT "(select '' from dual)",
TRANSFERTEXT "(select '' from dual)"
)