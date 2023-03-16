using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiTestLibrary.Interfaces
{
    public interface IServiceIntegrationTestBase
    {
        void Get_returns_401_Unauthorized_if_not_authenticated();

        void Get_returns_200_Authorized();

        void Get_All_returns_200_And_List_if_successful();

        void Get_All_204_No_Content();

        void Get_By_Id_returns_200_And_Item_if_successful();

        void Post_returns_201_if_successful();

        void Put_Returns_204_if_successful();

        void Patch_Returns_204_And_Item_if_successful();

        void Delete_Returns_204_if_successful();

        void Post_Returns_422_if_required_prop_is_missing();

        void Put_Returns_422_if_required_prop_is_missing();

        void GetByID_returns_404_If_given_Id_not_found();

        void Returns_BadRequestErrorMessageResult_when_route_Id_and_Model_Id_are_different();
    }
}
