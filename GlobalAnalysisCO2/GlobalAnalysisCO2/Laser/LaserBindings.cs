//-----------------------------------------------------------------------
// <copyright file="LaserBindings.cs" company="Brookfield Residential Properties">
//     Copyright (c) Brookfield Residential Properties. All rights reserved.
// </copyright>
// <author>Victor Procure</author>
//-----------------------------------------------------------------------
namespace GlobalAnalysisCO2.Laser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class LaserBindings : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
#if DEBUG
            base.Bind<ILaserController>().To<MockLaserController>();
#else
            base.Bind<ILaserController>().To<LaserController>();
#endif
        }
    }
}