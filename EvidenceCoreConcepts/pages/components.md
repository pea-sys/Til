---
title: Components
sidebar_position: 1
queries:
  - orders_with_comparisons.sql
  - sales_distribution_by_channel.sql
  - price_vs_volume.sql
  - categories.sql
  - orders_by_day_2021.sql
  - orders_by_day.sql
  - orders_by_category_2021.sql
---

```sql orders_by_month
select
    date_trunc('month', order_datetime) as month,
    count(*) as number_of_orders,
    sum(sales) as sales
from needful_things.orders
group by 1, order by 1 desc
```

# Accordion

<Accordion>
  <AccordionItem title="Item 1">

    This is the first item's accordion body.

    You can use **markdown** here too!
    Make sure to include an empty line after the component if you want to use markdown.

  </AccordionItem>
  <AccordionItem title="Item 2">

    This is the second item's accordion body with <b>bold text</b>.

  </AccordionItem>
  <AccordionItem title="Item 3">

    This is the third item's accordion body.

  </AccordionItem>
</Accordion>

# Alert

<Alert>
This is a default alert
</Alert>

<Alert status="info">
This is a informational alert
</Alert>

<Alert status="success">
This is a successful alert
</Alert>

<Alert status="warning">
This is a warning alert
</Alert>

<Alert status="danger">
This is a dangerous alert
</Alert>

# Annotations

<LineChart data={orders_by_month} x=month y=sales yFmt=usd0>
    <ReferenceLine y=7500 label="Reference Line" hideValue labelPosition="aboveStart" color=green/>
    <ReferenceArea xMin='2020-03-14' xMax='2020-08-15' label="Reference Area" color=yellow/>
    <ReferencePoint x="2019-07-01" y=6590 label="Reference Point" labelPosition=bottom color=red/>
    <Callout x="2021-05-01" y=11012 labelPosition=bottom labelWidth=fit>
        Callout
        Data trending up here
    </Callout>
</LineChart>

# Area Chart

<AreaChart 
    data={orders_by_month}
    x=month
    y=sales
/>

# Bar Chart

<BarChart 
    data={orders_by_month}
    x=month
    y=sales
/>

# Big Link

<BigLink href='/components/big-link/'>
  My Big Link
</BigLink>

# BigValues

<BigValue 
  data={queries.orders_with_comparisons} 
  value=num_orders
  sparkline=month
  comparison=order_growth
  comparisonFmt=pct1
  comparisonTitle="vs. Last Month"
/>

# BoxPlot

<BoxPlot 
    data={sales_distribution_by_channel}
    title="Daily Sales Distribution by Channel"
    name=channel
    intervalBottom=first_quartile
    midpoint=median
    intervalTop=third_quartile
    yFmt=usd0
/>

<BoxPlot 
    data={sales_distribution_by_channel}
    name=channel
    intervalBottom=first_quartile
    midpoint=median
    intervalTop=third_quartile
    yFmt=usd0
    swapXY=true
/>

# Bubble Chart

<BubbleChart 
    data={price_vs_volume}
    x=price
    y=number_of_units
    xFmt=usd0
    series=category
    size=total_sales
/>

# Button Group

<ButtonGroup
    data={categories} 
    name=category_button_group
    value=category
/>

```sql filtered_query
select
    category, item, sum(sales) as total_sales
from needful_things.orders
where category like '${inputs.category_button_group}'
group by all
```

# Calendar Heatmap

<CalendarHeatmap 
    data={orders_by_day_2021}
    date=day
    value=sales
    title="Calendar Heatmap"
    subtitle="Daily Sales"
/>

# Checkbox

<Checkbox
    title="Hide Months 0" 
    name=hide_months_0 
/>

# Data Table

<DataTable data={orders_by_day_2021}/>

# Date Range

<DateRange
    name=range_filtering_a_query
    data={orders_by_day}
    dates=day
/>

```sql filtered_query
select
    *
from ${orders_by_day}
where day between '${inputs.range_filtering_a_query.start}' and '${inputs.range_filtering_a_query.end}'
```

<LineChart
    data={filtered_query}
    x=day
    y=sales
/>

# Delta

```sql growth
select 0.366 as positive, -0.366 as negative
```

<Delta data={growth} column=positive fmt=pct1 />

# Detail

<Details title="Definitions">
    
    Definition of metrics in Solutions Targets

    ### Time to Proposal

    Average number of days it takes to create a proposal for a customer

    *Calculation:*
    Sum of the number of days it took to create each proposal, divided by the number of proposals created

    *Source:*
    Hubspot

</Details>

```orders

select state, category, item, channel, sales from needful_things.orders

```

# Dimension Grid

<DimensionGrid data={orders} metric='sum(sales)' name=selected_dimensions />

# DownloadData

<DownloadData data={categories}/>

# Dropdown

```sql categories
select distinct category as category_name, upper(left(category, 3)) as abbrev from needful_things.orders
```

<Dropdown 
    data={categories} 
    name=Category
    value=category_name 
    title="Select a Category" 
    defaultValue="Sinister Toys"
/>

```sql funnel_data
select * from (
    select 150 as customers, 'Show' as stage, 1 as stage_id
    union all
    select 102 as customers, 'Click' as stage, 2 as stage_id
    union all
    select 49 as customers, 'Visit' as stage, 3 as stage_id
    union all
    select 40 as customers, 'Inquiry' as stage, 4 as stage_id
    union all
    select 14 as customers, 'Order' as stage, 5 as stage_id
) order by stage_id asc
```

# FunnelChart

<FunnelChart 
    data={funnel_data} 
    nameCol=stage
    valueCol=customers
/>

# Grid

```sql orders_by_category
select order_month, count(1) as orders from needful_things.orders
group by all
```

<Grid cols=2>
    <LineChart data={orders_by_category} x=order_month y=orders/>
    <BarChart data={orders_by_category} x=order_month y=orders fillColor=#00b4e0/>
    <ScatterPlot data={orders_by_category} x=order_month y=orders fillColor=#015c08/>
    <AreaChart data={orders_by_category} x=order_month y=orders fillColor=#b8645e lineColor=#b8645e/>
</Grid>

# Heatmap

```orders
select category, dayname(order_datetime) as day, dayofweek(order_datetime) as day_num, count(*) as order_count from needful_things.orders
group by all
order by category, day_num
```

<Heatmap 
    data={orders} 
    x=day 
    y=category 
    value=order_count 
    valueFmt=usd 
/>
