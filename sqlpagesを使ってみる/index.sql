SELECT 'list' AS component, 'Popular websites' AS title;

SELECT 'Hello' AS title, 'world' AS description, 'https://wikipedia.org' AS link;
SELECT 'form' AS component, 'Add a user' AS title;
SELECT 'Username' as name, TRUE as required;
INSERT INTO users (name)
SELECT :Username
WHERE :Username IS NOT NULL;
SELECT 'list' AS component, 'Users' AS title;
SELECT name AS title,  name || ' is a user on this website.' as description FROM users;

SELECT 
    'table' as component,
    1 as sort,
    1 as search;
SELECT 
    'Ophir' as Forename,
    'Lojkine' as Surname,
    'lovasoa' as Pseudonym;
SELECT 
    'Linus' as Forename,
    'Torvalds' as Surname,
    'torvalds' as Pseudonym;
