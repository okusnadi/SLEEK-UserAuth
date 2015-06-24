/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHost.MR
{
    public class CustomRepository : DbContextUserAccountRepository<CustomDatabase, CustomUser>, IUserAccountRepository<CustomUser>
    {
        // you can do either style ctor (or none) -- depends how much control 
        // you want over instantiating the CustomDatabase instance
        public CustomRepository(CustomDatabase ctx)
            : base(ctx)
        {
        }

        protected override IQueryable<CustomUser> DefaultQueryFilter(IQueryable<CustomUser> query, string filter)
        {
            if (query == null) throw new ArgumentNullException("query");
            if (filter == null) throw new ArgumentNullException("filter");

            return
                from a in query
                from c in a.ClaimCollection
                where c.Value.Contains(filter)
                select a;
        }
    }
}