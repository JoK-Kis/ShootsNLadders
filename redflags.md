# Red flags

Known limitations and possible future fixes. Each item: what we chose, why, and when to revisit.

## Entry total is computed at runtime, not in the database

- **Now:** `Total` (physical + mental + emotional) is a read-only property on the `Entry` entity, calculated in C# when read. It is not stored in the database.
- **Why:** simplest option, can never get out of sync with the three ratings.
- **Limitation:** the database cannot sort, filter or aggregate by total by itself (e.g. "days where total < 12" must be done in C# after loading entries).
- **Possible fix:** add a SQLite generated column with EF Core `HasComputedColumnSql("Physical + Mental + Emotional", stored: false)` in one migration. The API response does not need to change.
- **Revisit when:** insights (step 6) or graph queries need to filter/sort by total in SQL, or data grows large enough that loading entries into memory is slow.

## "Future date" check uses UTC today + 1 day, not the user's own time zone

- **Now:** the server rejects an entry whose date is after (UTC today + 1 day). The one-day margin means a user in any time zone (max UTC+14) can always log their own "today".
- **Why:** users and their time zones don't exist until step 5.
- **Limitation:** someone can log "tomorrow" a few hours early, and the rule isn't exact for anyone.
- **Possible fix:** store a time zone per user (e.g. `Europe/Vilnius`) and compare against that user's local today, using `TimeProvider` + `TimeZoneInfo`.
- **Revisit when:** step 5 (accounts) adds users.

## Note length is not enforced by the database

- **Now:** `EntryConfiguration` uses `HasMaxLength(500)` for `Note`, but SQLite ignores text lengths, so only the API's DTO validation stops notes longer than 500 characters.
- **Why:** kept to the standard one-line EF Core rule.
- **Limitation:** anything that writes to the database without going through the DTO (a bug, a future background job, a manual script) can store longer notes.
- **Possible fix:** add a check constraint `length(Note) <= 500` in `EntryConfiguration` and a migration.
- **Revisit when:** anything other than the API starts writing entries, or before moving to production.

## Rating check is one combined constraint

- **Now:** one check constraint `CK_Entries_Ratings` covers Physical, Mental and Emotional together.
- **Limitation:** if it fails, the database error doesn't say which rating was out of range. (Users normally get the DTO's specific error first, so this only affects debugging.)
- **Possible fix:** split into `CK_Entries_Physical`, `CK_Entries_Mental`, `CK_Entries_Emotional` with a migration.
- **Revisit when:** a check-constraint failure is ever hard to debug.
