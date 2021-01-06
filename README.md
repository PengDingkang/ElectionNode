# ElectionNode

* You need .Net 5.0 SDK to run this program.

* Please manually add `app.config` to the root path, it may look like:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <add key="OtherNodes" value="http://localhost:PORT0,http://localhost:PORT1, ..."/>
    <add key="ListenPort" value="port of this node"/>
    <add key="NodeNumber" value="number of thi node"/>
    <add key="NodeAmount" value="amount of nodes"/>
  </appSettings>
</configuration>
```
