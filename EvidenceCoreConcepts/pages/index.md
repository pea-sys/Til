---
title: Evidence uses Markdown
description: Evidence uses Markdown to write expressively in text.
og:
  image: /my-social-image.png
---

# Markdown

---

## title: Evidence uses Markdown

Markdown can be used to write expressively in text.

- it supports lists,
- **bolding**, _italics_ and `inline code`,
- links to [external sites](https://google.com) and other [Evidence pages](/another/page)

## Images 🖼️

Evidence looks for images in your `static` folder, e.g. `static/my-logo.png`.
![Company Logo](/my-logo.png)

# Syntax

```sql orders_by_month
select
    date_trunc('month', order_datetime) as order_month,
    count(*) as number_of_orders,
    sum(sales) as sales_usd
from needful_things.orders
group by 1, order by 1 desc
```

# Component

<LineChart 
    data = {orders_by_month}    
    y = sales_usd 
    title = 'Sales by Month, USD' 
/>

# Loop

{#each orders_by_month as month}

- There were <Value data={month} column=number_of_orders/> orders in <Value data={month} />.

{/each}

# If / Else

{#if orders_by_month && orders_by_month.length > 1}
{#if orders_by_month[0].sales_usd > orders_by_month[1].sales_usd}

Sales are up month-over-month.

{:else}

Sales are down vs last month. See [category detail](/sales-by-category).

{/if}
{:else}

  <div>No sufficient data available to perform the comparison.</div>
{/if}

# Page Variables

The current page path is: {$page.route.id}

<!-- Result: The current page path is: /core-concepts/syntax/ -->

# Expression

2 + 2 = {2 + 2}

<!-- Result: 2 + 2 = 4 -->

There were {orders_by_month[0].number_of_orders} orders last month.

<!-- Result: There were 3634 orders last month. -->

# Code Fences in Other Languages

```python
names = ["Alice", "Bob", "Charlie"]
for name in names:
    print("Hello, " + name)
```

# Partials

{@partial "my-first-partial.md"}

And some content specific to this page.

# Postgres Access

```sql actors
select
    *
from dvdrental.actors
```

```sql databases
SELECT * FROM dvdrental.databases
```

# SQL Query

```sql sales_by_category
select
  category, sum(sales) as sales
from needful_things.orders
group by 1
```

<LineChart data={sales_by_category}/>

## Query Chaining

```sql sales_by_item
select
    item,
    sum(sales) as sales
from needful_things.orders
group by 1
```

```sql average_sales
select
    avg(sales) as average_sales
from ${sales_by_item}
```
