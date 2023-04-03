using System.Runtime.CompilerServices;

namespace CCCategoryService.Dtos.Enums
{
    /// <summary>
        /// The types used for category pools
        /// </summary>
    public enum CategoryPoolType
    {
        /// <summary>
                /// The pool type category (single select, e.g. Warengruppen)
                /// </summary>
        PoolTypeCategory = 1,         /// <summary>
                                              /// The pool type tags (multiple select, e.g. rebate)
                                              /// </summary>
        PoolTypeTags = 2,         /// <summary>
                                          /// The pool type tax (single select tax model)
                                          /// </summary>
        PoolTypeTax = 3,         /// <summary>
                                         /// The pool type menu plan (menu plan selections)
                                         /// </summary>
        PoolTypeMenuPlan = 4,

        
        
    }   
    
    

}