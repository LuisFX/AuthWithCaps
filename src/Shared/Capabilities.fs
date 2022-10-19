namespace Shared

module Capabilities =
    open Types
    open Shared
    open System.Collections.Generic

    // each access token gets its own type
    type AccessCustomer = AccessCustomer of CustomerId
    type UpdatePassword = UpdatePassword of CustomerId
    type AccssTodos = AccssTodos of User

    // capabilities
    type GetCustomerCap = unit -> Result<CustomerData,FailureCase>
    type UpdateCustomerCap = CustomerData -> Result<unit,FailureCase>
    type UpdatePasswordCap = Password -> Result<unit,FailureCase>
    type GetTodosCap = unit -> Result<string list,FailureCase>

    type CapabilityProvider = {
        /// given a customerId and User, attempt to get the GetCustomer capability
        getCustomer : CustomerId -> User -> GetCustomerCap option
        /// given a customerId and User, attempt to get the UpdateCustomer capability
        updateCustomer : CustomerId -> User -> UpdateCustomerCap option
        /// given a customerId and User, attempt to get the UpdatePassword capability
        updatePassword : CustomerId -> User -> UpdatePasswordCap option 
        /// get all todos for the given user
        getTodos : User -> GetTodosCap option
    }