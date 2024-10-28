---
title: Components
sidebar_position: 1
queries:
  - orders_by_day.sql
  - price_vs_volume.sql
  - state_population.sql
---

# Histgram

<Histogram
    data={orders_by_day} 
    x=day 
/>

# Last Refreshed

<LastRefreshed/>

# Line Chart

<LineChart 
    data={orders_by_day}
    x=day
    y=sales 
    yAxisTitle="Sales per Day"
/>

# Link Button

<LinkButton url='/components/link-button'>
  My Link Button
</LinkButton>

# Mixed-Type Charts

<Chart data={orders_by_day}>
    <Bar y=sales/>
    <Line y=aov/>
</Chart>

# Modal

<Modal title="Title" buttonText='Open Modal'>

Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

</Modal>

# Sankey Diagram

```sql simple_sankey
select 'products' as source, 'profits' as target, 100 as amount, 0.67 as percent
union all
select 'products' as source, 'expenses' as target, 50 as amount, 0.33 as percent
union all
select 'services' as source, 'profits' as target, 25 as amount, 0.50 as percent
union all
select 'services' as source, 'expenses' as target, 25 as amount, 0.50 as percent
```

```sql traffic_data
select 'google' as source, 'all_traffic' as target, 100 as count
union all
select 'direct' as source, 'all_traffic' as target, 50 as count
union all
select 'facebook' as source, 'all_traffic' as target, 25 as count
union all
select 'bing' as source, 'all_traffic' as target, 25 as count
union all
select 'tiktok' as source, 'all_traffic' as target, 25 as count
union all
select 'twitter' as source, 'all_traffic' as target, 25 as count
union all
select 'linkedin' as source, 'all_traffic' as target, 25 as count
union all
select 'pinterest' as source, 'all_traffic' as target, 25 as count
union all
select 'all_traffic' as source, '/' as target, 50 as count
union all
select 'all_traffic' as source, '/docs' as target, 150 as count
union all
select 'all_traffic' as source, '/blog' as target, 25 as count
union all
select 'all_traffic' as source, '/about' as target, 75 as count
```

<SankeyDiagram
data={traffic_data}
title="Sankey"
subtitle="A simple sankey chart"
sourceCol=source
targetCol=target
valueCol=count
echartsOptions={{
        title: {
            text: "Custom Echarts Option",
            textStyle: {
              color: '#476fff'
            }
        }
    }}
/>

```sql apple_income_statement
select 'iphone' as source, 'product revenue' as target, 51 as amount_usd
union all
select 'mac' as source, 'product revenue' as target, 10 as amount_usd
union all
select 'ipad' as source, 'product revenue' as target, 8 as amount_usd
union all
select 'wearables and home' as source, 'product revenue' as target, 9 as amount_usd
union all
select 'services revenue' as source, 'revenue' as target, 20 as amount_usd
union all
select 'product revenue' as source, 'revenue' as target, 78 as amount_usd
union all
select 'revenue' as source, 'gross profit' as target, 43 as amount_usd
union all
select 'gross profit' as source, 'operating profit' as target, 30 as amount_usd
union all
select 'gross profit' as source, 'operating expenses' as target, 13 as amount_usd
union all
select 'revenue' as source, 'cost of revenue' as target, 55 as amount_usd
```

<SankeyDiagram
data={apple_income_statement}
title="Apple Income Statement"
subtitle="USD Billions"
sourceCol=source
targetCol=target
valueCol=amount_usd
depthOverride={{'services revenue': 1}}
nodeAlign=left
/>

# Scatter Plot

<ScatterPlot 
    data={price_vs_volume}
    x=price
    y=number_of_units
    xFmt=usd0
    series=category
/>

# Slider

<Slider
    title="Months" 
    name=months
    defaultValue=18
/>

# Sparkline

```sql orders_by_month
select order_month as month, sum(sales) as sales_usd0k, count(1) as orders from needful_things.orders
group by all
```

<Sparkline 
    data={orders_by_month}
    dateCol=month
    valueCol=sales_usd0k 
    color=navy
/>

# Tabs

<Tabs>
    <Tab label="First Tab">
        Content of the First Tab

        You can use **markdown** here too!
    </Tab>
    <Tab label="Second Tab">
        Content of the Second Tab

        Here's a [link](https://www.google.com)
    </Tab>

</Tabs>

# Text Input

<TextInput
    name=name_of_input
    title="Search"
/>

# USMap

<USMap
    data={state_population}
    state=state_name
    value=population
/>

# Value

```sql simple_count
select sales from needful_things.orders
```

<Value data={simple_count} column="sales" agg="avg" fmt="usd0" color="#85BB65" />

#

```sql sales_by_country
select 'Canada' as country, 100 as sales
union all
select 'US' as country, 250 as sales
union all
select 'UK' as country, 130 as sales
union all
select 'Australia' as country, 95 as sales
```

```sql test_data
select country as name, sales as value
from ${sales_by_country}
```

<ECharts config={
{
title: {
text: 'Treemap Example',
left: 'center'
},
tooltip: {
formatter: '{b}: {c}'
},
series: [
{
type: 'treemap',
visibleMin: 300,
label: {
show: true,
formatter: '{b}'
},
itemStyle: {
borderColor: '#fff'
},
roam: false,
nodeClick: false,
data: [...test_data],
breadcrumb: {
show: false
}
}
]
}
}
/>
