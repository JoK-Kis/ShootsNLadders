# Red flags

Known limitations and possible future fixes. Each item: what we chose, why, and when to revisit.

## Entry total is computed at runtime, not in the database

- **Now:** `Total` (physical + mental + emotional) is a read-only property on the `Entry` entity, calculated in C# when read. It is not stored in the database.
- **Why:** simplest option, can never get out of sync with the three ratings.
- **Limitation:** the database cannot sort, filter or aggregate by total by itself (e.g. "days where total < 12" must be done in C# after loading entries).
- **Possible fix:** add a SQLite generated column with EF Core `HasComputedColumnSql("Physical + Mental + Emotional", stored: false)` in one migration. The API response does not need to change.
- **Revisit when:** insights (step 6) or graph queries need to filter/sort by total in SQL, or data grows large enough that loading entries into memory is slow.
