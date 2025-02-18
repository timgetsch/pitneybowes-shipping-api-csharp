﻿/*
Copyright 2019 Pitney Bowes Inc.

Licensed under the MIT License(the "License"); you may not use this file except in compliance with the License.  
You may obtain a copy of the License in the README file or at
   https://opensource.org/licenses/MIT 
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed 
on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the License 
for the specific language governing permissions and limitations under the License.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

using System.Threading.Tasks;
using PitneyBowes.Developer.ShippingApi.Json;

namespace PitneyBowes.Developer.ShippingApi
{
    public static partial class Api
    {
        /// <summary>
        /// This POST operation rates a package for one or more services before a shipment label is purchased and printed.
        ///
        /// Things to Consider:
        ///     * In order to rate a package for a single service, you must specify the rates.parcelType and rates.serviceId fields.
        ///     * In order to rate a package for multiple services (Rate Shopping) for a single parcel type, you must specify the 
        ///     rates.parcelType field and omit the rates.serviceId field.
        ///     * You can also find rates for multiple parcel types and services with one call by omitting both the rates.parcelType 
        ///     and rates.serviceId fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async static Task<ShippingApiResponse<T>> Rates<T>(T request, ISession session = null) where T : IShipment, new()
        {
            var ratesRequest = new JsonShipment<T>(request);
            return await WebMethod.Post<T, JsonShipment<T>>("/shippingservices/v1/rates", ratesRequest, session);
        }
    }
}
