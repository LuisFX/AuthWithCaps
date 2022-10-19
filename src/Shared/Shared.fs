namespace Shared

open System
module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

    let capabilityRouteBuilder _ methodName =
        printf "Route is: %s" methodName
        sprintf "/api/capability/%s" methodName