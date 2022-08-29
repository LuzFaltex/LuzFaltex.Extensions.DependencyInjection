// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using LuzFaltex.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

[RequiresDynamicCode(MutableServiceProvider.RequiresDynamicCodeMessage)]
internal sealed class IEnumerableCallSite : ServiceCallSite
{
    internal Type ItemType { get; }
    internal ServiceCallSite[] ServiceCallSites { get; }

    public IEnumerableCallSite(ResultCache cache, Type itemType, ServiceCallSite[] serviceCallSites) : base(cache)
    {
        ItemType = itemType;
        ServiceCallSites = serviceCallSites;
    }

    public override Type ServiceType => typeof(IEnumerable<>).MakeGenericType(ItemType);
    public override Type ImplementationType  => ItemType.MakeArrayType();
    public override CallSiteKind Kind { get; } = CallSiteKind.IEnumerable;
}
